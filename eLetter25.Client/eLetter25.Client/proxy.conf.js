// Reads the API base URL injected by .NET Aspire via WithReference(letterApi).
// Falls back to the local HTTP port for standalone runs without Aspire.
const aspireApiUrl =
  process.env['services__API__http__0'] ||
  process.env['services__API__https__0'];

const target = aspireApiUrl ?? 'http://localhost:5296';

console.log(`[proxy] /api → ${target}`);

module.exports = {
  '/api': {
    target,
    secure: false,
    changeOrigin: true,
    logLevel: 'warn'
  }
};

