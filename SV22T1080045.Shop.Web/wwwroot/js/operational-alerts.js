(() => {
    const config = window.operationalAlertsConfig;
    if (!config?.feedUrl) return;

    const storageKey = config.storageKey || 'operationalAlerts';
    const pollMs = config.pollIntervalMs || 30000;
    const soundKey = `${storageKey}:sound`;
    const soundStyleKey = `${storageKey}:soundStyle`;
    const soundVolumeKey = `${storageKey}:soundVolume`;

    let soundEnabled = localStorage.getItem(soundKey) === '1';
    let soundStyle = localStorage.getItem(soundStyleKey) || 'classic';
    let soundVolume = Number(localStorage.getItem(soundVolumeKey) || '35');
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

    const styleSelect = document.querySelector('[data-sound-style]');
    if (styleSelect) {
        styleSelect.value = soundStyle;
        styleSelect.addEventListener('change', () => {
            soundStyle = styleSelect.value || 'classic';
            localStorage.setItem(soundStyleKey, soundStyle);
            if (soundEnabled) playTone('info');
        });
    }

    const volumeInput = document.querySelector('[data-sound-volume]');
    if (volumeInput) {
        volumeInput.value = String(Number.isFinite(soundVolume) ? soundVolume : 35);
        volumeInput.addEventListener('input', () => {
            const next = Number(volumeInput.value);
            soundVolume = Number.isFinite(next) ? Math.max(0, Math.min(100, next)) : 35;
            localStorage.setItem(soundVolumeKey, String(soundVolume));
        });
        volumeInput.addEventListener('change', () => {
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

    function tonePreset(style, kind) {
        const presets = {
            classic: { info: 520, warning: 660, danger: 880, durInfo: 0.22, durWarn: 0.26, durDanger: 0.35 },
            soft: { info: 440, warning: 554, danger: 740, durInfo: 0.18, durWarn: 0.22, durDanger: 0.28 },
            sharp: { info: 620, warning: 820, danger: 1040, durInfo: 0.16, durWarn: 0.20, durDanger: 0.26 },
        };
        const p = presets[style] || presets.classic;
        const freq = kind === 'danger' ? p.danger : kind === 'warning' ? p.warning : p.info;
        const dur = kind === 'danger' ? p.durDanger : kind === 'warning' ? p.durWarn : p.durInfo;
        return { freq, dur };
    }

    function playTone(kind) {
        if (!soundEnabled) return;
        try {
            const ctx = new (window.AudioContext || window.webkitAudioContext)();
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();
            osc.connect(gain);
            gain.connect(ctx.destination);
            const preset = tonePreset(soundStyle, kind);
            osc.frequency.value = preset.freq;
            const vol = Math.max(0, Math.min(100, Number.isFinite(soundVolume) ? soundVolume : 35)) / 100;
            gain.gain.value = 0.12 * vol;
            osc.start();
            osc.stop(ctx.currentTime + preset.dur);
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
