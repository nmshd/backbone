import Koa from 'koa';
import { CoreBuffer, CryptoPasswordGenerator, CryptoSignaturePrivateKey, CryptoSignatures } from '@nmshd/crypto';
import koaBody from 'koa-body';
import Router from 'koa-router';

const app = new Koa();
const router = new Router();
const port = 3000;

app.use(
    koaBody({
        jsonLimit: '1kb'
    })
);

app.use(async (ctx, next) => {
    const start = Date.now();
    await next();
    const ms = Date.now() - start;
    console.log(`${ctx.method} ${ctx.url} - ${ms} ms`);
});

router.get('/keypair', async (ctx) => {
    const keypair = await CryptoSignatures.generateKeypair();
    ctx.response.body = keypair.toJSON();
    ctx.response.status = 200;
    ctx.response.type = 'application/json';
});

router.get('/password', async (ctx) => {
    const password = await CryptoPasswordGenerator.createPasswordWithBitStrength();
    ctx.response.body = password;
    ctx.response.status = 200;
});

router.post('/sign', async (ctx) => {
    const challenge = CoreBuffer.fromUtf8(ctx.request.body.challenge);
    const privateKey = ctx.request.body.keyPair.prv;

    const cryptoSignaturePrivateKey = CryptoSignaturePrivateKey.fromJSON({prv: privateKey.prv, alg: privateKey.alg});

    const signedChallenge = await CryptoSignatures.sign(challenge, cryptoSignaturePrivateKey);

    ctx.response.body = signedChallenge.toJSON();
    ctx.response.status = 200;
});

app.use(router.routes());

app.listen(port, () => {
    console.log(`Sidecar server is listening on port ${port}`);
});
