import http from "k6/http";

const host = __ENV.HOST;

export function getJwt() {
    const bodyConnectToken = {
        client_id: "test",
        client_secret: __ENV.CLIENT_SECRET,
        username: __ENV.USERNAME,
        password: __ENV.PASSWORD,
        grant_type: "password"
    };

    const authToken = http.post(
        `${host}/connect/token`,
        bodyConnectToken,
        {
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            }
        }
    ).json("access_token")!
    .toString();

    return authToken
}