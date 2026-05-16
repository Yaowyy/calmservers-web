(function () {
  // Load nav.html into #nav-placeholder
  fetch('/nav.html')
    .then(r => r.text())
    .then(html => {
      document.getElementById('nav-placeholder').outerHTML = html;

      // Highlight active link based on current page filename
      const page = location.pathname.split('/').pop() || 'index.html';
      document.querySelectorAll('#site-nav .nav-links a[data-page]').forEach(a => {
        if (a.dataset.page && page.startsWith(a.dataset.page)) {
          a.classList.add('active');
        }
      });
    });
})();
