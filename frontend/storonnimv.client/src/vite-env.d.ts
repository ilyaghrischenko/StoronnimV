/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_API_URL: string;
}

interface ImportMeta {
    readonly env: ImportMetaEnv;
}

svgr({
    // svgr options: https://react-svgr.com/docs/options/
    svgrOptions: {
        // ...
    },

    // esbuild options, to transform jsx to js
    esbuildOptions: {
        // ...
    },

    // A minimatch pattern, or array of patterns, which specifies the files in the build the plugin should include.
    include: "**/*.svg?react",

    //  A minimatch pattern, or array of patterns, which specifies the files in the build the plugin should ignore. By default no files are ignored.
    exclude: "",
});
