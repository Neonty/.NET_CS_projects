document.documentElement.classList.add('is-loading');

window.addEventListener('load', function () {
    document.documentElement.classList.remove('is-loading');
});

document.addEventListener('DOMContentLoaded', function () {
    var loader = document.getElementById('page-loader');
    if (!loader) return;

    function showLoader() {
        loader.classList.remove('hidden');
    }

    document.querySelectorAll('a[href]:not([target="_blank"])').forEach(function (link) {
        link.addEventListener('click', function (e) {
            if (e.ctrlKey || e.metaKey || e.shiftKey) return;
            var href = link.getAttribute('href');
            if (!href || href.startsWith('#') || href.startsWith('javascript')) return;
            showLoader();
        });
    });

    document.querySelectorAll('form').forEach(function (form) {
        form.addEventListener('submit', function () {
            showLoader();
        });
    });
});
