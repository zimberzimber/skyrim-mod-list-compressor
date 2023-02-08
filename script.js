
function OpenAll() {
    [].slice.call(document.querySelectorAll(".nexus-mod"), 0).reverse().forEach(a => window.open(a.href));
}