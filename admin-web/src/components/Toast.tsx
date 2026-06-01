import type { ReactNode } from 'react'
import { toast as sonner } from 'sonner'
import { Toaster } from './ui/sonner'

export function ToastProvider({ children }: { children: ReactNode }) {
  return (
    <>
      {children}
      <Toaster richColors position="top-right" />
    </>
  )
}

export function useToast() {
  return {
    success: (msg: string) => sonner.success(msg),
    error: (msg: string) => sonner.error(msg),
    info: (msg: string) => sonner.message(msg),
  }
}
