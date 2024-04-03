# Svelte + Vite

Interface of the application

## How to start / use

Check in navigator :
```
npm install
npm run dev
```

Update the UI of the C++ app : 
```
npm run build
```
Clean the CMake project and rebuild.

## C++ Interop

`src/lib/cpp.js` is used to "expose" the C++ methods, make the IDE happy with syntaxe.
`src/lib/global.js` define functions used from C++.