// Arcane Vault — motion & interaction engine.
// Progressive enhancement: every feature checks for its dependencies and
// leaves the page fully usable when JS, GSAP, or motion preference is absent.

(function () {
    "use strict";

    var reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    var hasGsap = typeof gsap !== "undefined";

    /* ---------------------------------------------------------------
       Non-motion enhancements (run regardless of GSAP / motion pref)
       --------------------------------------------------------------- */

    // Highlight the current page in the navbar.
    (function markActiveNav() {
        var path = window.location.pathname.replace(/\/$/, "") || "/";
        document.querySelectorAll(".navbar .nav-link[href]").forEach(function (link) {
            var href = link.getAttribute("href").replace(/\/$/, "") || "/";
            var active = href === "/" ? path === "/" : path.indexOf(href) === 0;
            if (active) {
                link.classList.add("active");
                link.setAttribute("aria-current", "page");
            }
        });
    })();

    // Password visibility toggles.
    document.querySelectorAll(".password-toggle").forEach(function (btn) {
        btn.addEventListener("click", function () {
            var input = btn.parentElement.querySelector("input");
            if (!input) return;
            var show = input.type === "password";
            input.type = show ? "text" : "password";
            btn.setAttribute("aria-pressed", show ? "true" : "false");
            btn.querySelector(".eye-open").style.display = show ? "none" : "";
            btn.querySelector(".eye-closed").style.display = show ? "" : "none";
        });
    });

    // Lift server-rendered alerts into a floating toast stack (aria-live),
    // auto-dismissing successes. Without JS the alerts simply stay inline.
    (function toasts() {
        var alerts = document.querySelectorAll("main .alert[data-toast]");
        if (!alerts.length) return;
        var stack = document.createElement("div");
        stack.className = "toast-stack";
        stack.setAttribute("aria-live", "polite");
        document.body.appendChild(stack);
        alerts.forEach(function (alert) {
            stack.appendChild(alert);
            if (hasGsap && !reducedMotion) {
                gsap.from(alert, { x: 40, autoAlpha: 0, duration: 0.45, ease: "power3.out" });
            }
            if (alert.classList.contains("alert-success")) {
                setTimeout(function () {
                    if (hasGsap && !reducedMotion) {
                        gsap.to(alert, {
                            x: 40, autoAlpha: 0, duration: 0.35, ease: "power2.in",
                            onComplete: function () { alert.remove(); }
                        });
                    } else {
                        alert.remove();
                    }
                }, 4200);
            }
        });
    })();

    // Navbar condenses after scrolling begins.
    (function navShrink() {
        var nav = document.querySelector(".navbar");
        if (!nav) return;
        var update = function () {
            nav.classList.toggle("is-scrolled", window.scrollY > 24);
        };
        update();
        window.addEventListener("scroll", update, { passive: true });
    })();

    if (!hasGsap || reducedMotion) return;

    /* ---------------------------------------------------------------
       Motion layer (GSAP). ScrollTrigger/SplitText register if present.
       --------------------------------------------------------------- */

    var hasScrollTrigger = typeof ScrollTrigger !== "undefined";
    var hasSplitText = typeof SplitText !== "undefined";
    if (hasScrollTrigger) gsap.registerPlugin(ScrollTrigger);
    if (hasSplitText) gsap.registerPlugin(SplitText);

    var EASE = "expo.out";

    // --- Page curtain: reveal on load, wipe on internal navigation ---
    var curtain = document.querySelector(".page-curtain");
    if (curtain) {
        gsap.set(curtain, { visibility: "visible", yPercent: 0 });
        gsap.to(curtain, {
            yPercent: -101,
            duration: 0.7,
            ease: "expo.inOut",
            delay: 0.05,
            onComplete: function () { gsap.set(curtain, { visibility: "hidden", yPercent: 101 }); }
        });

        // Restore state when a page is served from the back/forward cache.
        window.addEventListener("pageshow", function (e) {
            if (e.persisted) gsap.set(curtain, { visibility: "hidden", yPercent: 101 });
        });

        document.addEventListener("click", function (e) {
            if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
            var link = e.target.closest("a[href]");
            if (!link) return;
            var href = link.getAttribute("href");
            if (!href || href.charAt(0) === "#" || link.target === "_blank" || link.hasAttribute("download")) return;
            if (link.origin !== window.location.origin) return;
            // Same-page anchors and the page we're already on shouldn't wipe.
            if (link.pathname === window.location.pathname && link.search === window.location.search) return;
            e.preventDefault();
            gsap.set(curtain, { visibility: "visible", yPercent: 101 });
            gsap.to(curtain, {
                yPercent: 0,
                duration: 0.5,
                ease: "expo.inOut",
                onComplete: function () { window.location.href = link.href; }
            });
        });
    }

    // --- Hero choreography ---
    var hero = document.querySelector(".hero");
    if (hero) {
        var heroBg = hero.querySelector(".hero-bg");
        if (heroBg) {
            gsap.to(heroBg, { scale: 1.08, duration: 22, ease: "sine.inOut", repeat: -1, yoyo: true });
        }

        var heroIntro = function () {
            var h1 = hero.querySelector("h1");
            var tl = gsap.timeline({ defaults: { ease: EASE } });
            tl.from(hero.querySelector(".hero-eyebrow"), { y: 24, autoAlpha: 0, duration: 0.7 }, 0.35);
            if (h1 && hasSplitText) {
                var split = new SplitText(h1, { type: "words", mask: "words" });
                tl.from(split.words, {
                    yPercent: 115, duration: 1.0, stagger: 0.07
                }, 0.45);
            } else if (h1) {
                tl.from(h1, { y: 30, autoAlpha: 0, duration: 0.9 }, 0.45);
            }
            tl.from(hero.querySelector(".lead"), { y: 24, autoAlpha: 0, duration: 0.8 }, "-=0.55");
            tl.from(hero.querySelectorAll(".hero-actions > *"), { y: 18, autoAlpha: 0, duration: 0.6, stagger: 0.08 }, "-=0.5");
            tl.from(hero.querySelector(".scroll-cue"), { autoAlpha: 0, duration: 0.6 }, "-=0.3");
        };
        // Split lines only after fonts load, so line breaks are measured correctly.
        if (document.fonts && document.fonts.ready) {
            document.fonts.ready.then(heroIntro);
        } else {
            heroIntro();
        }

        // Cinematic scrub: hero content drifts and fades as you scroll past it.
        if (hasScrollTrigger) {
            gsap.to(hero.querySelector(".hero-content"), {
                yPercent: -12,
                autoAlpha: 0.25,
                ease: "none",
                scrollTrigger: { trigger: hero, start: "bottom 80%", end: "bottom 30%", scrub: true }
            });
        }
    }

    // --- Scroll reveals ---
    if (hasScrollTrigger) {
        // Single elements.
        gsap.utils.toArray("[data-reveal]").forEach(function (el) {
            gsap.from(el, {
                y: 36, autoAlpha: 0, duration: 0.9, ease: EASE,
                scrollTrigger: { trigger: el, start: "top 86%", once: true }
            });
        });

        // Groups: children stagger in together.
        gsap.utils.toArray("[data-reveal-group]").forEach(function (group) {
            gsap.from(group.children, {
                y: 30, autoAlpha: 0, duration: 0.7, ease: EASE, stagger: 0.08,
                scrollTrigger: { trigger: group, start: "top 85%", once: true }
            });
        });

        // Parallax media (showcase image drifts inside its frame).
        gsap.utils.toArray("[data-parallax] img").forEach(function (img) {
            gsap.fromTo(img, { yPercent: -7 }, {
                yPercent: 7, ease: "none",
                scrollTrigger: { trigger: img.parentElement, start: "top bottom", end: "bottom top", scrub: true }
            });
        });

        // Count-up numbers when they enter the viewport.
        gsap.utils.toArray("[data-counter]").forEach(function (el) {
            var target = parseFloat(el.getAttribute("data-counter"));
            if (isNaN(target)) return;
            var obj = { v: 0 };
            gsap.to(obj, {
                v: target, duration: 1.6, ease: "power3.out",
                scrollTrigger: { trigger: el, start: "top 90%", once: true },
                onUpdate: function () { el.textContent = Math.round(obj.v).toLocaleString(); }
            });
        });
    } else {
        // No ScrollTrigger: simple entrance for above-the-fold groups.
        gsap.from("[data-reveal], [data-reveal-group] > *", {
            y: 24, autoAlpha: 0, duration: 0.6, ease: EASE, stagger: 0.05
        });
    }

    // --- Generic page entrance for pages without a hero ---
    if (!hero) {
        var main = document.querySelector("main");
        if (main) gsap.from(main, { y: 14, autoAlpha: 0, duration: 0.55, ease: EASE, delay: 0.15 });
    }
})();
