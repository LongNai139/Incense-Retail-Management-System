(() => {
    const config = window.operationalAlertsConfig;
    if (!config?.feedUrl) return;

    const storageKey = config.storageKey || 'operationalAlerts';
    const pollMs = config.pollIntervalMs || 30000;
    const soundKey = `${storageKey}:sound`;

    let soundEnabled = localStorage.getItem(soundKey) === '1';
    let lastPollAt = localStorage.getItem(`${storageKey}:since`);
    const seenKeys = new Set(JSON.parse(localStorage.getItem(`${storageKey}:seen`) || '[]'));

    const toastHost = document.createElement('div');
    toastHost.className = 'ops-alert-host';
    document.body.appendChild(toastHost);

    const soundButton = document.querySelector('[data-sound-toggle]');
    if (soundButton) {
        renderSoundButton();
        soundButton.addEventListener('click', () => {
            soundEnabled = !soundEnabled;
            localStorage.setItem(soundKey, soundEnabled ? '1' : '0');
            renderSoundButton();
            if (soundEnabled) playTone('info');
        });
    }

    function renderSoundButton() {
        if (!soundButton) return;
        soundButton.classList.toggle('primary', soundEnabled);
        soundButton.innerHTML = soundEnabled
            ? '<i class="fa-solid fa-volume-high"></i> Âm báo: Bật'
            : '<i class="fa-solid fa-volume-xmark"></i> Âm báo: Tắt';
    }

    function playTone(kind) {
        if (!soundEnabled) return;
        try {
            const ctx = new (window.AudioContext || window.webkitAudioContext)();
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();
            osc.connect(gain);
            gain.connect(ctx.destination);
            osc.frequency.value = kind === 'danger' ? 880 : kind === 'warning' ? 660 : 520;
            gain.gain.value = 0.08;
            osc.start();
            osc.stop(ctx.currentTime + (kind === 'danger' ? 0.35 : 0.22));
        } catch {
            // Ignore if browser blocks audio.
        }
    }

    function persistSeen() {
        localStorage.setItem(`${storageKey}:seen`, JSON.stringify(Array.from(seenKeys).slice(-120)));
    }

    function showToast(alert) {
        const toast = document.createElement('div');
        toast.className = `ops-alert-toast ops-alert-${alert.kind}`;
        toast.innerHTML = `<strong>${alert.title}</strong><span>${alert.message}</span>`;
        if (alert.targetAnchor) {
            toast.style.cursor = 'pointer';
            toast.addEventListener('click', () => {
                const target = document.getElementById(alert.targetAnchor);
                if (!target) return;
                target.scrollIntoView({ behavior: 'smooth', block: 'center' });
                target.classList.add('pulse');
                window.setTimeout(() => target.classList.remove('pulse'), 1400);
            });
        }
        toastHost.appendChild(toast);
        window.setTimeout(() => toast.classList.add('show'), 10);
        window.setTimeout(() => {
            toast.classList.remove('show');
            window.setTimeout(() => toast.remove(), 300);
        }, 8000);
    }

    function toneForKind(kind) {
        if (kind === 'late_order' || kind === 'cancelled') return 'danger';
        if (kind === 'low_stock') return 'warning';
        return 'info';
    }

    async function poll() {
        try {
            const url = new URL(config.feedUrl, window.location.origin);
            if (lastPollAt) url.searchParams.set('since', lastPollAt);

            const response = await fetch(url, { credentials: 'same-origin' });
            if (!response.ok) return;

            const data = await response.json();
            let played = false;

            (data.alerts || []).forEach(alert => {
                if (seenKeys.has(alert.key)) return;
                seenKeys.add(alert.key);
                showToast(alert);
                if (!played) {
                    playTone(toneForKind(alert.kind));
                    played = true;
                }
            });

            persistSeen();
            lastPollAt = data.serverTime;
            localStorage.setItem(`${storageKey}:since`, lastPollAt);
        } catch {
            // Silent retry on next interval.
        }
    }

    poll();
    window.setInterval(poll, pollMs);
})();
