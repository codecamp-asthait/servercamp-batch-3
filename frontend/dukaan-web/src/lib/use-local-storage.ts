import { useSyncExternalStore } from "react";

function subscribe(callback: () => void) {
  window.addEventListener("storage", callback);
  return () => window.removeEventListener("storage", callback);
}

const getSnapshot = (key: string) => () => localStorage.getItem(key);
const getServerSnapshot = () => undefined;

/**
 * Returns:
 *  - `undefined` on the server (not hydrated yet)
 *  - `null` on the client when key is absent
 *  - `string` on the client when key exists
 */
export function useLocalStorageToken(key: string): string | null | undefined {
  return useSyncExternalStore(
    subscribe,
    getSnapshot(key),
    getServerSnapshot
  );
}
