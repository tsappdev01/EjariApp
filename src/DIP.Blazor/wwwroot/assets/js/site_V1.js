window.triggerFileInput = (id) => {
    const element = document.getElementById(id);
    if (element) {
        element.click();
    } else {
        console.error("File input not found for id:", id);
    }
};

// Uploads the selected file as a plain multipart HTTP POST so the file bytes never
// travel over the Blazor SignalR circuit. The initial read (arrayBuffer) runs with a
// timeout so an unreadable file (e.g. a cloud-only placeholder) fails fast with a
// clear error code instead of hanging.
window.uploadNocDocument = async (inputId, url, fields, readTimeoutMs) => {
    const input = document.getElementById(inputId);
    const file = input && input.files ? input.files[0] : null;
    if (!file) {
        return { success: false, errorCode: "NoFile", attachmentName: null };
    }

    let buffer;
    try {
        buffer = await Promise.race([
            file.arrayBuffer(),
            new Promise((_, reject) =>
                setTimeout(() => reject(new Error("ReadTimeout")), readTimeoutMs || 30000))
        ]);
    } catch (err) {
        console.error("uploadNocDocument: could not read file:", err);
        return {
            success: false,
            errorCode: err && err.message === "ReadTimeout" ? "ReadTimeout" : "ReadError",
            attachmentName: null
        };
    }

    try {
        const formData = new FormData();
        formData.append("file", new Blob([buffer], { type: file.type }), file.name);
        for (const key in fields) {
            formData.append(key, fields[key] ?? "");
        }

        const headers = {};
        const xsrfMatch = document.cookie.match(/(?:^|;\s*)XSRF-TOKEN=([^;]*)/);
        if (xsrfMatch) {
            headers["RequestVerificationToken"] = decodeURIComponent(xsrfMatch[1]);
        }

        const response = await fetch(url, { method: "POST", body: formData, headers: headers });
        if (!response.ok) {
            console.error("uploadNocDocument: server returned", response.status);
            return { success: false, errorCode: "Network", attachmentName: null };
        }
        return await response.json();
    } catch (err) {
        console.error("uploadNocDocument: network/upload failure:", err);
        return { success: false, errorCode: "Network", attachmentName: null };
    }
};
