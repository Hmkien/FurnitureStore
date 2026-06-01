import { type ReactNode } from 'react'

export function PageHeader({ title, action }: { title: string; action?: ReactNode }) {
  return (
    <div className="mb-4 flex items-center justify-between">
      <h1 className="text-xl font-semibold text-slate-900">{title}</h1>
      {action}
    </div>
  )
}

export function SearchBar({
  value,
  onChange,
  placeholder = 'Tìm kiếm...',
}: {
  value: string
  onChange: (v: string) => void
  placeholder?: string
}) {
  return (
    <input
      className="input max-w-xs"
      value={value}
      placeholder={placeholder}
      onChange={(e) => onChange(e.target.value)}
    />
  )
}

export function TableWrap({ children }: { children: ReactNode }) {
  return (
    <div className="overflow-hidden rounded-xl bg-white shadow-sm ring-1 ring-slate-200">
      <div className="overflow-x-auto">
        <table className="w-full text-sm">{children}</table>
      </div>
    </div>
  )
}

export function Th({ children, className = '' }: { children?: ReactNode; className?: string }) {
  return (
    <th className={`bg-slate-50 px-4 py-3 text-left font-medium text-slate-500 ${className}`}>
      {children}
    </th>
  )
}

export function Td({ children, className = '' }: { children?: ReactNode; className?: string }) {
  return <td className={`border-t border-slate-100 px-4 py-3 align-middle ${className}`}>{children}</td>
}

export function Field({
  label,
  children,
}: {
  label: string
  children: ReactNode
}) {
  return (
    <div className="mb-3">
      <label className="label">{label}</label>
      {children}
    </div>
  )
}

export function EmptyRow({ colSpan, loading }: { colSpan: number; loading?: boolean }) {
  return (
    <tr>
      <td colSpan={colSpan} className="border-t border-slate-100 px-4 py-10 text-center text-slate-400">
        {loading ? 'Đang tải...' : 'Không có dữ liệu'}
      </td>
    </tr>
  )
}
