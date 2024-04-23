const Koa = require("koa");
const crypto = require("@nmshd/crypto");
const koaBody = require("koa-body").koaBody;

const app = new Koa();

app.use(
    koaBody({
        jsonLimit: "1kb"
    })
);

// response
app.use(async function (ctx, next) {
    if (ctx.method !== "GET" || ctx.path !== "/keypair") return await next();

    const keypair = await crypto.CryptoSignatures.generateKeypair();

    ctx.body = JSON.stringify(keypair, null, 2);
    ctx.status = 200;
});

app.use(async function (ctx, next) {
    if (ctx.method !== "POST" || ctx.path !== "/sign") return await next();
    const body = ctx.request.body;
    const challenge = new crypto.CoreBuffer(body.challenge.id);
    const privateKey = body.keyPair.prv;

    const cspk = crypto.CryptoSignaturePrivateKey.fromJSON({ privateKey: privateKey.prv, algorithm: privateKey.alg });

    const signedChallenge = await crypto.CryptoSignatures.sign(challenge, cspk);

    ctx.body = signedChallenge.toJSON();
    ctx.status = 200;
});

app.listen(3000);
