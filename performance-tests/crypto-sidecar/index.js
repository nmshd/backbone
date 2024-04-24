const Koa = require("koa");
const crypto = require("@nmshd/crypto");
const koaBody = require("koa-body").koaBody;

const app = new Koa();

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

app.use(async function (ctx, next) {
    if (ctx.method !== "GET" || ctx.path !== "/keypair") return await next();

    ctx.body = JSON.stringify(await crypto.CryptoSignatures.generateKeypair());
    ctx.status = 200;
});

app.use(async function (ctx, next) {
    if (ctx.method !== "GET" || ctx.path !== "/password") return await next();

    ctx.body = await crypto.CryptoPasswordGenerator.createPasswordWithBitStrength();
    ctx.status = 200;
});

app.use(async function (ctx, next) {
    if (ctx.method !== "POST" || ctx.path !== "/sign") return await next();
    const body = ctx.request.body;

    const challenge = crypto.CoreBuffer.fromUtf8(body.challenge);
    const privateKey = body.keyPair.prv;

    const cspk = crypto.CryptoSignaturePrivateKey.fromJSON({ privateKey: privateKey.prv, algorithm: privateKey.alg });

    const signedChallenge = await crypto.CryptoSignatures.sign(challenge, cspk);

    ctx.body = signedChallenge.toJSON();
    ctx.status = 200;
});

app.listen(3000);
