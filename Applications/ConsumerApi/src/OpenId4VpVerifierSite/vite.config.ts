import { defineConfig } from "vite";

export default defineConfig({
  base: "/openid4vp-verifier/",
  build: {
    assetsDir: "assets",
    chunkSizeWarningLimit: 5000,
    cssCodeSplit: false,
    emptyOutDir: true,
    lib: {
      entry: "src/main.ts",
      formats: ["es"]
    },
    outDir: "../wwwroot/openid4vp-verifier",
    rollupOptions: {
      output: {
        assetFileNames: (assetInfo) => (assetInfo.name?.endsWith(".css") ? "assets/verifier.css" : "assets/[name][extname]"),
        entryFileNames: "assets/verifier.js"
      }
    },
    sourcemap: false
  }
});
