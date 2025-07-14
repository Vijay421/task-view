import { useState, type FormEvent, type MouseEvent, useRef, type Dispatch, type SetStateAction } from "react";
import { Link } from "react-router";
import styles from "./LoginPage.module.scss";

type FieldStatus = {
    emailIsValid: boolean | null;
    passwordIsValid: boolean | null;
};

function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [fieldStatus, setFieldStatus] = useState<FieldStatus>({ emailIsValid: null, passwordIsValid: null });
    const form = useRef<HTMLFormElement>(null);

    const handleEmail = (e: FormEvent<HTMLInputElement>) => setEmail(e.currentTarget.value);
    const handlePassword = (e: FormEvent<HTMLInputElement>) => setPassword(e.currentTarget.value);

    const loginClick = (e: MouseEvent<HTMLButtonElement>) => {
        const button = e.currentTarget;
        pressLoginButton(button);

        const isValid = validate(email, password, setFieldStatus);
        if (isValid)
            login(email, password);
    };

    const emailClass = `${styles["errorText"]} ${fieldStatus.emailIsValid === false && styles["errorTextVisible"]}`;
    const passwordClass = `${styles["errorText"]} ${fieldStatus.passwordIsValid === false && styles["errorTextVisible"]}`;

    return (
        <main className="page">
            <section className={styles.loginSection}>
                <h1 className={styles.title}>Login</h1>

                <form ref={form} className={styles.form} onSubmit={preventDefault}>
                    <div className={styles["inputGroup"]}>
                        <input type="email" placeholder="E-mail" pattern="" onChange={handleEmail}/>
                        <p className={emailClass}>Enter your e-mail address</p>
                    </div>

                    <div className={styles["inputGroup"]}>
                        <input type="password" placeholder="Password" onChange={handlePassword}/>
                        <p className={passwordClass}>Enter your password</p>
                    </div>

                    <button className={styles.loginButton} onClick={loginClick}>Login</button>

                    <Link to="/login" className={styles.forgotPasswordLink}>Forgot password</Link>
                </form>
            </section>
        </main>
    );
}

/**
 * Prevents the default form submission behavior.
 *
 * @param e - The form event triggered on submit.
 */
function preventDefault(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
}

/**
 * Simulates a button press effect (like the CSS :active state) for keyboard users.
 * 
 * This function temporarily adds a CSS class to the given button element to visually
 * mimic a press interaction when activated via the keyboard (e.g., Enter key). It reads
 * the transition duration from the button's `--transition-duration` CSS variable to ensure
 * the timing matches the CSS animation.
 *
 * @param button - The HTML button element to apply the visual press effect to.
 */
function pressLoginButton(button: HTMLButtonElement) {
    button.classList.add(styles["loginButtonActive"]);

    const style = getComputedStyle(button);
    const timeStr = style.getPropertyValue("--transition-duration").trim();
    const time = timeStr.endsWith("ms")
        ? parseFloat(timeStr)
        : parseFloat(timeStr) * 1000;
    setTimeout(() => button.classList.remove(styles["loginButtonActive"]), time);
}

/**
 * Validates the given email and password by checking that they are non-empty.
 * Updates the corresponding field validity status.
 *
 * @param email - The email string to validate.
 * @param password - The password string to validate.
 * @param setFieldStatus - A state setter function that updates the validity of each field.
 * 
 * @returns `true` if both the email and password are non-empty; otherwise, `false`.
 */
function validate(email: string, password: string, setFieldStatus: Dispatch<SetStateAction<FieldStatus>>): boolean {
    const isEmailValid = email.length !== 0;
    const isPasswordValid = password.length !== 0;

    setFieldStatus(old => ({ ...old, emailIsValid: email.length !== 0 }));
    setFieldStatus(old => ({ ...old, passwordIsValid: password.length !== 0 }));

    return isEmailValid && isPasswordValid;
}

async function login(email: string, password: string) {
    try {
        const response = await fetch("api/v1/auth/login?useCookies=true&useSessionCookies=true", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Email: email,
                Password: password,
            }),
        });
    
        if (response.ok) {
            console.log("Login successful!");
            return "Login successful!";
        } else {
            try {
                const errMsg = await response.json();
                console.error("Server error: ", errMsg);
            } catch {
                console.error("Could not parse the server error");
            }

            throw new Error("Incorrect credentials");
        }
    } catch (err) {
        // if (err.message === "Incorrect credentials") {
        //     throw err;
        // }

        throw new Error("Could not reach the server");
    }
}

export default LoginPage;
