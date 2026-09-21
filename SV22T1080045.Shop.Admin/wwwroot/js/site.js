// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.querySelectorAll('.fade-up').forEach((element) => {
    if (!('IntersectionObserver' in window)) {
        element.classList.add('visible');
        return;
    }

    const observer = new IntersectionObserver((entries, currentObserver) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                currentObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12 });

    observer.observe(element);
});

document.querySelectorAll('[data-nav-dropdown]').forEach((dropdown) => {
    const toggle = dropdown.querySelector('.nav-dropdown-toggle');
    if (!toggle) return;

    function close() {
        dropdown.classList.remove('open');
        toggle.setAttribute('aria-expanded', 'false');
    }

    function open() {
        dropdown.classList.add('open');
        toggle.setAttribute('aria-expanded', 'true');
    }

    toggle.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        dropdown.classList.contains('open') ? close() : open();
    });

    document.addEventListener('click', (e) => {
        if (!dropdown.classList.contains('open')) return;
        if (dropdown.contains(e.target)) return;
        close();
    });

    document.addEventListener('keydown', (e) => {
        if (e.key !== 'Escape') return;
        if (!dropdown.classList.contains('open')) return;
        close();
    });
});
