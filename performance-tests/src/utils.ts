/**
 * Generates a random number between {@link min} and {@link max} inclusive.
 * @returns integer
 */
export function randomIntFromInterval(min: number, max: number): number {
    return Math.floor(Math.random() * (max - min + 1) + min);
}
