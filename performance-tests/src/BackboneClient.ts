import { CoreBuffer, CryptoSignatureKeypair } from "@nmshd/crypto";
import {
    AbstractAuthenticator,
    Challenge,
    ChallengeAuthClient,
    ChallengeClient,
    ChallengeSigned,
    ChallengeType,
    CoreCrypto,
    CoreDate,
    CoreId,
    CredentialsBasic,
    DeviceAuthClient,
    FileClient,
    IRESTClientConfig,
    IdentityClient,
    RelationshipClient,
    RelationshipTemplateClient,
    SyncClient,
    TokenClient
} from "@nmshd/transport";

export class BackboneClient {
    private static readonly DEFAULT_PASSWORD = "test";
    private static readonly CLIENT_ID = "test";
    private static readonly CLIENT_SECRET = "test";

    public readonly relationshipTemplates: RelationshipTemplateClient;
    public readonly challenges: ChallengeAuthClient;
    public readonly devices: DeviceAuthClient;
    public readonly files: FileClient;
    public readonly relationships: RelationshipClient;
    public readonly sync: SyncClient;
    public readonly tokens: TokenClient;

    private constructor(
        private readonly config: IRESTClientConfig,
        authenticator: Authenticator,
        private readonly signatureKeypair: CryptoSignatureKeypair
    ) {
        this.challenges = new ChallengeAuthClient(config, authenticator);
        this.relationshipTemplates = new RelationshipTemplateClient(config, authenticator);
        this.devices = new DeviceAuthClient(config, authenticator);
        this.files = new FileClient(config, authenticator);
        this.relationships = new RelationshipClient(config, authenticator);
        this.sync = new SyncClient({ ...config, supportedDatawalletVersion: 1 }, authenticator);
        this.tokens = new TokenClient(config, authenticator);
    }

    public static async initWithNewIdentity(config: BackboneClientConfig): Promise<BackboneClient> {
        const signatureKeypair = await CoreCrypto.generateSignatureKeypair();

        const restClientConfig = this.translateConfig(config);
        const identity = await this.createIdentity(signatureKeypair, restClientConfig);

        const authenticator = new Authenticator(restClientConfig, identity.value.device.username, this.DEFAULT_PASSWORD);
        const client = new BackboneClient(restClientConfig, authenticator, signatureKeypair);

        return client;
    }

    public static async initFromExistingClient(existingClient: BackboneClient, username: string, password: string): Promise<BackboneClient> {
        const restClientConfig = BackboneClient.translateConfig(existingClient.config);

        const client = new BackboneClient(restClientConfig, new Authenticator(restClientConfig, username, password), existingClient.signatureKeypair);

        return client;
    }

    private static translateConfig(config: BackboneClientConfig): IRESTClientConfig {
        return {
            baseUrl: config.baseUrl,
            platformClientId: config.platformClientId ?? "test",
            platformClientSecret: config.platformClientId ?? "test",
            debug: true,
            platformMaxRedirects: 5,
            platformTimeout: 10000,
            httpAgent: {
                keepAlive: true,
                maxSockets: 5,
                maxFreeSockets: 2
            },
            httpsAgent: {
                keepAlive: true,
                maxSockets: 5,
                maxFreeSockets: 2
            },
            platformAdditionalHeaders: {}
        };
    }

    private static async createIdentity(signatureKeypair: CryptoSignatureKeypair, config: IRESTClientConfig) {
        const identityClient = new IdentityClient(config);

        const signedChallenge = await this.createSignedChallenge(signatureKeypair, config);

        const identity = await identityClient.createIdentity({
            clientId: this.CLIENT_ID,
            clientSecret: this.CLIENT_SECRET,
            devicePassword: this.DEFAULT_PASSWORD,
            identityPublicKey: signatureKeypair.publicKey.toBase64(),
            identityVersion: 1,
            signedChallenge: signedChallenge.toJSON(false)
        });
        return identity;
    }

    private static async createSignedChallenge(signatureKeypair: CryptoSignatureKeypair, config: IRESTClientConfig) {
        const challengeClient = new ChallengeClient(config);
        const backboneResponse = (await challengeClient.createChallenge()).value;
        const challenge = Challenge.from({
            id: CoreId.from(backboneResponse.id),
            expiresAt: CoreDate.from(backboneResponse.expiresAt),
            type: ChallengeType.Identity
        });
        const serializedChallenge = challenge.serialize(false);
        const challengeBuffer = CoreBuffer.fromUtf8(serializedChallenge);
        const signature = await CoreCrypto.sign(challengeBuffer, signatureKeypair.privateKey);
        const signedChallenge = ChallengeSigned.from({
            challenge: serializedChallenge,
            signature: signature
        });
        return signedChallenge;
    }

    public async createNewClientForNewDevice() {
        const signedChallenge = await BackboneClient.createSignedChallenge(this.signatureKeypair, this.config);
        const newDevice = await this.devices.createDevice({ devicePassword: "test", signedChallenge: signedChallenge.toJSON(false) });

        const backboneClient2 = await BackboneClient.initFromExistingClient(this, newDevice.value.username, "test");

        return backboneClient2;
    }
}

export class Authenticator extends AbstractAuthenticator {
    public constructor(
        config: IRESTClientConfig,
        private readonly username: string,
        private readonly password: string
    ) {
        super(config);
    }

    protected getCredentials(): Promise<CredentialsBasic> {
        return Promise.resolve({ username: this.username, password: this.password });
    }
}

export interface BackboneClientConfig {
    baseUrl: string;
    platformClientId: string;
    platformClientSecret: string;
}
