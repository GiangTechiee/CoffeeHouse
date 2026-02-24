import DOMPurify from 'dompurify';

/**
 * Sanitizes HTML content to prevent XSS attacks.
 * Uses DOMPurify under the hood.
 */
export const sanitizeHtml = (html: string): string => {
    return DOMPurify.sanitize(html, {
        ALLOWED_TAGS: ['b', 'i', 'em', 'strong', 'a', 'p', 'br', 'ul', 'ol', 'li'],
        ALLOWED_ATTR: ['href', 'target', 'rel', 'class'],
    });
};

/**
 * Hook-like utility for sanitizing in components if needed,
 * though direct utility is usually sufficient.
 */
export const useSanitize = () => {
    return { sanitizeHtml };
};
