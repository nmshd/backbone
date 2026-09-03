import { defineConfig } from "vite";

export default defineConfig({
  base: "/openid4vp-verifier/",
  build: {
    assetsDir: "assets",
    chunkSizeWarningLimit: 5000,
    emptyOutDir: true,
    outDir: "../wwwroot/openid4vp-verifier",
    sourcemap: false
  }
});
