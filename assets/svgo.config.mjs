export default {
    plugins: [
        {
            name: 'preset-default',
            params: {
                overrides: {
                    cleanupIds: true,
                    inlineStyles: {
                        onlyMatchedOnce: false,
                    },
                },
            },
        },
        'convertStyleToAttrs',
        'cleanupNumericValues',
        'removeUnknownsAndDefaults',
    ],
}
