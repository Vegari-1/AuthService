module.exports = {
	branches: ["main", "develop"],
	repositoryUrl: "https://github.com/Vegari-1/AuthService",
	plugins: [
		"@semantic-release/commit-analyzer", // analizira komit poruke i odredjuje narednu verziju
		"@semantic-release/release-notes-generator", // generise release notes na osnovu komita
		"@semantic-release/github" // kreira github release
	]
}