﻿using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.Crypto;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;

public class SignedChallenge
{
    public SignedChallenge(string challenge, ConvertibleString signature)
    {
        Challenge = challenge;
        Signature = ConvertibleString.FromUtf8(
            JsonConvert.SerializeObject(new CryptoSignatureSignedChallenge { alg = CryptoHashAlgorithm.SHA512, sig = signature.BytesRepresentation }
        )).Base64Representation;
    }

    public string Challenge { get; internal set; }
    public string Signature { get; internal set; }
}
