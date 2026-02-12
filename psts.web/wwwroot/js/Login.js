(function () {
    // Robust Enter-key handling for the local login form.
    var form = document.getElementById('local-login-form');
    if (!form) return;

    // Submit the local login form when Enter is pressed inside any of its text/password inputs.
    form.addEventListener('keydown', function (e) {
        // Only respond to Enter
        if (e.key !== 'Enter') return;

        // Find the element that currently has focus
        var active = document.activeElement;
        if (!active) return;

        // Ignore Enter on buttons (they naturally submit) or on textareas
        var tag = active.tagName;
        if (tag === 'BUTTON' || tag === 'TEXTAREA') return;

        // Ensure the focused element is inside our form
        if (!form.contains(active)) return;

        // Use requestSubmit() when available to respect form validation and submit handlers
        if (typeof form.requestSubmit === 'function') {
            form.requestSubmit();
        } else {
            form.submit();
        }

        // Prevent default to avoid duplicate submits in some browsers
        e.preventDefault();
    });
})();