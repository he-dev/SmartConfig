// convertMd.js
var marked = require('marked')
var fs = require('fs')

const inFile = process.argv[2]
const outFile = process.argv[3]

const inputContent = fs.readFileSync(inFile, 'utf8')

const content = `
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Title</title>
</head>
<body>
${marked(inputContent)}
</body>
</html>`

fs.writeFileSync(outFile, content)