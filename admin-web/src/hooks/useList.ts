import { useCallback, useEffect, useState } from 'react'
import type { BaseQuery, PagedResult } from '../types'
import { getErrorMessage } from '../api/client'
import { useToast } from '../components/Toast'

type Fetcher<T> = (q: BaseQuery) => Promise<PagedResult<T>>

/** Hook quản lý danh sách phân trang + tìm kiếm cho các trang CRUD. */
export function useList<T>(fetcher: Fetcher<T>, pageSize = 10, searchIn?: string[]) {
  const toast = useToast()
  const [items, setItems] = useState<T[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [keyword, setKeyword] = useState('')
  const [loading, setLoading] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await fetcher({
        pageNumber: page,
        pageSize,
        keyword: keyword || undefined,
        searchIn,
        sortBy: 'Created',
        sortDesc: true,
      })
      setItems(res.data ?? [])
      setTotal(res.recordsFiltered ?? 0)
    } catch (e) {
      toast.error(getErrorMessage(e, 'Không tải được dữ liệu'))
    } finally {
      setLoading(false)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, keyword])

  useEffect(() => {
    load()
  }, [load])

  return { items, total, page, setPage, keyword, setKeyword, loading, reload: load, pageSize }
}
