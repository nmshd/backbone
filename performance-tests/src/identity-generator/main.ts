import { BackboneClient } from "../BackboneClient";

const main = ()=>{
    const config = {
        baseUrl: "http://localhost:8081",
        platformClientId: "test",
        platformClientSecret: "test"
    };

    const client = BackboneClient.initWithNewIdentity(config);
    console.log(client);
};

main();