(() => {
  const reduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  const targets = document.querySelectorAll('.glass-card, .card, .alert, .table-responsive');

  if (!reduced && targets.length) {
    targets.forEach((el, i) => {
      el.classList.add('oc-reveal');
      el.style.transitionDelay = `${Math.min(i * 28, 220)}ms`;
    });

    if ('IntersectionObserver' in window) {
      const io = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add('in');
            io.unobserve(entry.target);
          }
        });
      }, { threshold: 0.14, rootMargin: '0px 0px -8% 0px' });

      targets.forEach((el) => io.observe(el));
    } else {
      targets.forEach((el) => el.classList.add('in'));
    }
  }

  if (reduced) return;

  const orbs = document.querySelectorAll('.orb');
  if (!orbs.length) return;

  let rafId = 0;
  const handleMove = (e) => {
    if (rafId) cancelAnimationFrame(rafId);
    rafId = requestAnimationFrame(() => {
      const x = (e.clientX / window.innerWidth - 0.5) * 18;
      const y = (e.clientY / window.innerHeight - 0.5) * 18;

      orbs.forEach((orb, idx) => {
        const factor = (idx + 1) * 0.35;
        orb.style.transform = `translate(${x * factor}px, ${y * factor}px)`;
      });
    });
  };

  window.addEventListener('mousemove', handleMove, { passive: true });
})();