import { build } from 'esbuild'
import { mkdirSync, copyFileSync } from 'fs'

const name = 'WebSharper.JQuery.Sample'

// The binding imports jQuery from the `jquery` npm package, so esbuild bundles
// it into the sample script (resolved from node_modules).
await build({
  entryPoints: [`./build/${name}.js`],
  bundle: true,
  minify: true,
  format: 'iife',
  outfile: `../dist/Scripts/${name}.min.js`,
  globalName: 'wsbundle'
})

mkdirSync('../dist/css', { recursive: true })
copyFileSync('./css/index.css', '../dist/css/index.css')
copyFileSync('./index.html', '../dist/index.html')
copyFileSync('./data.json', '../dist/data.json')

console.log('Bundled:', name)
