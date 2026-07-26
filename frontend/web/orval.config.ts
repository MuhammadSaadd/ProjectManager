import { defineConfig } from "orval";

export default defineConfig({
  api: {
    input: "../../schemas/api.json",
    output: {
      target: "./src/lib/api/generated/api.ts",
      client: "react-query",
      mode: "tags-split",
      schemas: "./src/lib/api/generated/schemas",
    },
  },
});
