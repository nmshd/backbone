"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.Authenticator = exports.BackboneClient = void 0;
var crypto_1 = require("@nmshd/crypto");
var transport_1 = require("@nmshd/transport");
var BackboneClient = /** @class */ (function () {
    function BackboneClient(config, authenticator, signatureKeypair) {
        this.config = config;
        this.signatureKeypair = signatureKeypair;
        this.challenges = new transport_1.ChallengeAuthClient(config, authenticator);
        this.relationshipTemplates = new transport_1.RelationshipTemplateClient(config, authenticator);
        this.devices = new transport_1.DeviceAuthClient(config, authenticator);
        this.files = new transport_1.FileClient(config, authenticator);
        this.relationships = new transport_1.RelationshipClient(config, authenticator);
        this.sync = new transport_1.SyncClient(__assign(__assign({}, config), { supportedDatawalletVersion: 1 }), authenticator);
        this.tokens = new transport_1.TokenClient(config, authenticator);
    }
    BackboneClient.initWithNewIdentity = function (config) {
        return __awaiter(this, void 0, void 0, function () {
            var signatureKeypair, restClientConfig, identity, authenticator, client;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, transport_1.CoreCrypto.generateSignatureKeypair()];
                    case 1:
                        signatureKeypair = _a.sent();
                        restClientConfig = this.translateConfig(config);
                        return [4 /*yield*/, this.createIdentity(signatureKeypair, restClientConfig)];
                    case 2:
                        identity = _a.sent();
                        authenticator = new Authenticator(restClientConfig, identity.value.device.username, this.DEFAULT_PASSWORD);
                        client = new BackboneClient(restClientConfig, authenticator, signatureKeypair);
                        return [2 /*return*/, client];
                }
            });
        });
    };
    BackboneClient.initFromExistingClient = function (existingClient, username, password) {
        return __awaiter(this, void 0, void 0, function () {
            var restClientConfig, client;
            return __generator(this, function (_a) {
                restClientConfig = BackboneClient.translateConfig(existingClient.config);
                client = new BackboneClient(restClientConfig, new Authenticator(restClientConfig, username, password), existingClient.signatureKeypair);
                return [2 /*return*/, client];
            });
        });
    };
    BackboneClient.translateConfig = function (config) {
        var _a, _b;
        return {
            baseUrl: config.baseUrl,
            platformClientId: (_a = config.platformClientId) !== null && _a !== void 0 ? _a : "test",
            platformClientSecret: (_b = config.platformClientId) !== null && _b !== void 0 ? _b : "test",
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
    };
    BackboneClient.createIdentity = function (signatureKeypair, config) {
        return __awaiter(this, void 0, void 0, function () {
            var identityClient, signedChallenge, identity;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        identityClient = new transport_1.IdentityClient(config);
                        return [4 /*yield*/, this.createSignedChallenge(signatureKeypair, config)];
                    case 1:
                        signedChallenge = _a.sent();
                        return [4 /*yield*/, identityClient.createIdentity({
                                clientId: this.CLIENT_ID,
                                clientSecret: this.CLIENT_SECRET,
                                devicePassword: this.DEFAULT_PASSWORD,
                                identityPublicKey: signatureKeypair.publicKey.toBase64(),
                                identityVersion: 1,
                                signedChallenge: signedChallenge.toJSON(false)
                            })];
                    case 2:
                        identity = _a.sent();
                        return [2 /*return*/, identity];
                }
            });
        });
    };
    BackboneClient.createSignedChallenge = function (signatureKeypair, config) {
        return __awaiter(this, void 0, void 0, function () {
            var challengeClient, backboneResponse, challenge, serializedChallenge, challengeBuffer, signature, signedChallenge;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        challengeClient = new transport_1.ChallengeClient(config);
                        return [4 /*yield*/, challengeClient.createChallenge()];
                    case 1:
                        backboneResponse = (_a.sent()).value;
                        challenge = transport_1.Challenge.from({
                            id: transport_1.CoreId.from(backboneResponse.id),
                            expiresAt: transport_1.CoreDate.from(backboneResponse.expiresAt),
                            type: transport_1.ChallengeType.Identity
                        });
                        serializedChallenge = challenge.serialize(false);
                        challengeBuffer = crypto_1.CoreBuffer.fromUtf8(serializedChallenge);
                        return [4 /*yield*/, transport_1.CoreCrypto.sign(challengeBuffer, signatureKeypair.privateKey)];
                    case 2:
                        signature = _a.sent();
                        signedChallenge = transport_1.ChallengeSigned.from({
                            challenge: serializedChallenge,
                            signature: signature
                        });
                        return [2 /*return*/, signedChallenge];
                }
            });
        });
    };
    BackboneClient.prototype.createNewClientForNewDevice = function () {
        return __awaiter(this, void 0, void 0, function () {
            var signedChallenge, newDevice, backboneClient2;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, BackboneClient.createSignedChallenge(this.signatureKeypair, this.config)];
                    case 1:
                        signedChallenge = _a.sent();
                        return [4 /*yield*/, this.devices.createDevice({ devicePassword: "test", signedChallenge: signedChallenge.toJSON(false) })];
                    case 2:
                        newDevice = _a.sent();
                        return [4 /*yield*/, BackboneClient.initFromExistingClient(this, newDevice.value.username, "test")];
                    case 3:
                        backboneClient2 = _a.sent();
                        return [2 /*return*/, backboneClient2];
                }
            });
        });
    };
    BackboneClient.DEFAULT_PASSWORD = "test";
    BackboneClient.CLIENT_ID = "test";
    BackboneClient.CLIENT_SECRET = "test";
    return BackboneClient;
}());
exports.BackboneClient = BackboneClient;
var Authenticator = /** @class */ (function (_super) {
    __extends(Authenticator, _super);
    function Authenticator(config, username, password) {
        var _this = _super.call(this, config) || this;
        _this.username = username;
        _this.password = password;
        return _this;
    }
    Authenticator.prototype.getCredentials = function () {
        return Promise.resolve({ username: this.username, password: this.password });
    };
    return Authenticator;
}(transport_1.AbstractAuthenticator));
exports.Authenticator = Authenticator;
