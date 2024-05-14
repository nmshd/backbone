const Koa = require("koa");
const crypto = require("@nmshd/crypto");
const koaBody = require("koa-body").koaBody;
const router = require("koa-router")();
const app = new Koa();

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

router.get("/keypair", async (ctx) => {
    ctx.body = (await crypto.CryptoSignatures.generateKeypair()).toJSON();
    ctx.status = 200;
    ctx.type = "application/json";
});

router.get("/password", async (ctx) => {
    ctx.body = await crypto.CryptoPasswordGenerator.createPasswordWithBitStrength();
    ctx.status = 200;
});

router.post("/sign", async (ctx) => {
    const body = ctx.request.body;

    const challenge = crypto.CoreBuffer.fromUtf8(body.challenge);
    const privateKey = body.keyPair.prv;

    const cspk = crypto.CryptoSignaturePrivateKey.fromJSON({ privateKey: privateKey.prv, algorithm: privateKey.alg });

    const signedChallenge = await crypto.CryptoSignatures.sign(challenge, cspk);

    ctx.body = signedChallenge.toJSON();
    ctx.status = 200;
});

app.use(router.routes());

app.listen(port);

console.log(`Sidecar server is listening on port ${port}`);
