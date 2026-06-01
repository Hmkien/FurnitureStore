import { createContext, useCallback, useContext, useState, type ReactNode } from 'react'

type Kind = 'success' | 'error' | 'info'
interface T { id: number; kind: Kind; msg: string }
interface Api { success: (m: string) => void; error: (m: string) => void; info: (m: string) => void }

const Ctx = createContext<Api | undefined>(undefined)
let counter = 0

export function ToastProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<T[]>([])
  const push = useCallback((kind: Kind, msg: string) => {
    const id = ++counter
    setItems((s) => [...s, { id, kind, msg }])
    setTimeout(() => setItems((s) => s.filter((x) => x.id !== id)), 3500)
  }, [])
  const api: Api = { success: (m) => push('success', m), error: (m) => push('error', m), info: (m) => push('info', m) }
  const color: Record<Kind, string> = { success: 'bg-emerald-600', error: 'bg-red-600', info: 'bg-zinc-800' }
  return (
    <Ctx.Provider value={api}>
      {children}
      <div className="fixed bottom-4 right-4 z-[100] flex flex-col gap-2">
        {items.map((t) => (
          <div key={t.id} className={`${color[t.kind]} rounded-lg px-4 py-2 text-sm text-white shadow-lg`}>{t.msg}</div>
        ))}
      </div>
    </Ctx.Provider>
  )
}

export function useToast() {
  const c = useContext(Ctx)
  if (!c) throw new Error('useToast must be inside ToastProvider')
  return c
}
