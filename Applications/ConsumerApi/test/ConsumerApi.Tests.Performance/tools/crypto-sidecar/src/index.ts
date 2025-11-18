import Router from "@koa/router";
import { CoreBuffer, CryptoSignaturePrivateKey, CryptoSignatures } from "@nmshd/crypto";
import Koa from "koa";
import koaBody from "koa-body";

const app = new Koa();
const router = new Router();
const port = 3000;

app.use(
    koaBody({
        jsonLimit: "1kb"
    })
);

app.use(async (ctx, next) => {
    const start = Date.now();
    await next();
    const ms = Date.now() - start;
    console.log(`${ctx.method} ${ctx.url} - ${ms} ms`);
});

router.get("/keypair", async ({ response }) => {
    const keypair = await CryptoSignatures.generateKeypair();
    response.body = keypair.toJSON();
    response.status = 200;
    response.type = "application/json";
});

router.post("/sign", async ({ request, response }) => {
    const body = request.body as any;
    const challenge = CoreBuffer.fromUtf8(body.challenge);
    const privateKey = body.keyPair.prv;
    const cryptoSignaturePrivateKey = CryptoSignaturePrivateKey.fromJSON({ prv: privateKey.prv, alg: privateKey.alg });

    const signedChallenge = await CryptoSignatures.sign(challenge, cryptoSignaturePrivateKey);

    response.body = signedChallenge.toJSON();
    response.status = 200;
});

app.use(router.routes());

app.listen(port, () => {
    console.log(`Sidecar server is listening on port ${port}`);
});
