import { type FormEvent } from "react";

/**
 * Prevents the default form submission behavior.
 *
 * @param e - The form event triggered on submit.
 */
export default function preventDefault(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
}
