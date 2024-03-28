import { CreateDatawalletModificationsRequestItem } from "@nmshd/transport";
import { randomBytes } from "crypto";

/**
/**
 * Generates a random number between min and max inclusive.
 * @param min minimum value
 * @param max maximum value
 * @returns random integer
 */
export function randomIntFromInterval(min: number, max: number): number {
    return Math.floor(Math.random() * (max - min + 1) + min);
}

/**
 * Generates a list of datawallet modifications.
 * @param numberOfModifications number of modifications
 * @param sizeOfModifications size of each modification in bytes
 * @returns list of generated datawallet modifications
 */
export function generateDataWalletModifications(numberOfModifications: number, sizeOfModifications: number): CreateDatawalletModificationsRequestItem[] {
    const userDataWalletModifications: CreateDatawalletModificationsRequestItem[] = [];

    for (let i = 0; i < numberOfModifications; i++) {
        const payload = randomBytesAsBase64String(sizeOfModifications);
        userDataWalletModifications.push({
            objectIdentifier: "test",
            collection: "Userdata",
            type: "Create",
            encryptedPayload: payload,
            datawalletVersion: 1
        });
    }
    return userDataWalletModifications;
}

export function randomBytesAsBase64String(sizeOfModifications: number) {
    return Buffer.from(randomBytes(sizeOfModifications)).toString("base64");
}

/**
 * Waits for the specified number of milliseconds.
 * @param milis the number of milliseconds to wait
 */
export async function sleep(milis: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, milis));
}
