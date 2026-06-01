import { useEffect, useState } from 'react'
import { Plus, Pencil, Trash2, Check, Ban, Package, Star } from 'lucide-react'
import { productsApi, categoriesApi, variantsApi, imagesApi } from '@/api/services'
import { getErrorMessage, API_URL } from '@/api/client'
import { useToast } from '@/components/Toast'
import SyncButton from '@/components/SyncButton'
import { MohoSeedButton } from '@/components/seed-buttons'
import MediaPicker from '@/components/MediaPicker'
import DataTable, { type Column } from '@/components/DataTable'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import RichTextEditor from '@/components/RichTextEditor'
import { cn } from '@/lib/utils'
import { currency, conditionLabel, enumOptions, mediaUrl } from '@/lib/format'
import { StatusEntity, ProductCondition, type Product, type Category, type ProductVariant, type ProductImage } from '@/types'

const statusMeta: Record<StatusEntity, { text: string; cls: string }> = {
  [StatusEntity.Approved]: { text: 'Đã duyệt', cls: 'bg-emerald-100 text-emerald-700' },
  [StatusEntity.Pending]: { text: 'Chờ duyệt', cls: 'bg-amber-100 text-amber-700' },
  [StatusEntity.Rejected]: { text: 'Đã hủy duyệt', cls: 'bg-red-100 text-red-700' },
  [StatusEntity.Draft]: { text: 'Nháp', cls: 'bg-slate-100 text-slate-600' },
}

const emptyVariant = { skuVariant: '', size: '', material: '', color: '', condition: ProductCondition.New, price: 0, stockQuantity: 0 }

export default function ProductsPage() {
  const toast = useToast()
  const [data, setData] = useState<Product[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [keyword, setKeyword] = useState('')
  const [loading, setLoading] = useState(false)
  const [cats, setCats] = useState<Category[]>([])

  const [open, setOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [form, setForm] = useState<Record<string, unknown>>({})
  const [variants, setVariants] = useState<ProductVariant[]>([])
  const [images, setImages] = useState<ProductImage[]>([])
  const [vForm, setVForm] = useState<Record<string, unknown>>({ ...emptyVariant })
  const [vEditId, setVEditId] = useState<string | null>(null)
  const [picker, setPicker] = useState(false)
  const [saving, setSaving] = useState(false)
  const pageSize = 10

  const load = async () => {
    setLoading(true)
    try {
      const res = await productsApi.paged({ pageNumber: page, pageSize, keyword: keyword || undefined, searchIn: ['Name', 'Sku'], sortBy: 'Created', sortDesc: true })
      setData(res.data ?? [])
      setTotal(res.recordsFiltered ?? 0)
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setLoading(false)
    }
  }
  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, keyword])

  useEffect(() => {
    categoriesApi.paged({ pageNumber: 1, pageSize: 200, sortDesc: true }).then((r) => setCats((r.data as Category[]).filter((c) => c.status === StatusEntity.Approved)))
  }, [])

  const catName = (id: string) => cats.find((c) => c.id === id)?.name ?? '—'

  const openCreate = () => {
    setEditingId(null)
    setForm({ categoryId: '', name: '', slug: '', sku: '', shortDescription: '', longDescription: '', style: '' })
    setVariants([])
    setImages([])
    setOpen(true)
  }
  const openEdit = async (p: Product) => {
    setEditingId(p.id)
    setOpen(true)
    try {
      const d = await productsApi.detail(p.id)
      setForm({ categoryId: d.categoryId, name: d.name, slug: d.slug, sku: d.sku, shortDescription: d.shortDescription ?? '', longDescription: d.longDescription ?? '', style: d.style ?? '' })
      setVariants(d.variants)
      setImages(d.images)
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const reloadChildren = async () => {
    if (!editingId) return
    setVariants(await variantsApi.byProduct(editingId))
    setImages(await imagesApi.byProduct(editingId))
  }

  const saveProduct = async () => {
    if (!form.categoryId) return toast.error('Chọn danh mục')
    if (!form.name || !form.sku) return toast.error('Nhập tên và SKU')
    setSaving(true)
    try {
      if (editingId) {
        await productsApi.update(editingId, form)
        toast.success('Đã lưu sản phẩm')
      } else {
        await productsApi.create(form)
        toast.success('Đã tạo. Mở lại để thêm biến thể & ảnh.')
        setOpen(false)
      }
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setSaving(false)
    }
  }

  const saveVariant = async () => {
    if (!vForm.skuVariant) return toast.error('Nhập SKU biến thể')
    try {
      const payload = { ...vForm, productId: editingId ?? undefined }
      if (vEditId) await variantsApi.update(vEditId, payload)
      else await variantsApi.create(payload)
      toast.success('Đã lưu biến thể')
      setVForm({ ...emptyVariant })
      setVEditId(null)
      reloadChildren()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }
  const delVariant = async (v: ProductVariant) => {
    if (!window.confirm('Xóa biến thể?')) return
    await variantsApi.remove(v.id).then(reloadChildren).catch((e) => toast.error(getErrorMessage(e)))
  }
  const addImage = async (url: string) => {
    try {
      await imagesApi.create({ productId: editingId ?? undefined, imageUrl: url, isPrimary: images.length === 0 })
      reloadChildren()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }
  const delImage = async (img: ProductImage) => {
    await imagesApi.remove(img.id).then(reloadChildren).catch((e) => toast.error(getErrorMessage(e)))
  }

  const act = async (fn: () => Promise<unknown>, ok: string) => {
    try {
      await fn()
      toast.success(ok)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const columns: Column<Product>[] = [
    { header: 'STT', className: 'w-14 text-center', cell: (_p, i) => <span className="text-muted-foreground">{(page - 1) * pageSize + i + 1}</span> },
    { header: 'Tên', cell: (p) => <span className="font-medium">{p.name}</span> },
    {
      header: 'Ảnh',
      className: 'w-20',
      cell: (p) => (p.thumbnailUrl ? <img src={mediaUrl(p.thumbnailUrl)} className="h-10 w-10 rounded-md border object-cover" /> : <div className="flex h-10 w-10 items-center justify-center rounded-md bg-muted"><Package className="h-4 w-4 text-muted-foreground" /></div>),
    },
    { header: 'SKU', cell: (p) => p.sku },
    { header: 'Danh mục', cell: (p) => catName(p.categoryId) },
    { header: 'Giá từ', cell: (p) => (p.minPrice != null ? currency(p.minPrice) : '—') },
    { header: 'Trạng thái', cell: (p) => <span className={cn('inline-flex rounded-full px-2 py-0.5 text-xs font-medium', statusMeta[p.status].cls)}>{statusMeta[p.status].text}</span> },
    {
      header: '',
      className: 'text-right',
      cell: (p) => (
        <div className="flex justify-end gap-1">
          {p.status === StatusEntity.Approved ? (
            <Button variant="ghost" size="icon" title="Hủy duyệt" onClick={() => act(() => productsApi.reject(p.id), 'Đã hủy duyệt')}><Ban className="h-4 w-4" /></Button>
          ) : (
            <>
              <Button variant="ghost" size="icon" title="Duyệt" className="text-emerald-600" onClick={() => act(() => productsApi.approve(p.id), 'Đã duyệt')}><Check className="h-4 w-4" /></Button>
              <Button variant="ghost" size="icon" title="Sửa" onClick={() => openEdit(p)}><Pencil className="h-4 w-4" /></Button>
              <Button variant="ghost" size="icon" title="Xóa" className="text-red-600" onClick={() => window.confirm('Xóa sản phẩm?') && act(() => productsApi.remove(p.id), 'Đã xóa')}><Trash2 className="h-4 w-4" /></Button>
            </>
          )}
        </div>
      ),
    },
  ]

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-xl font-semibold">Sản phẩm</h1>
        <div className="flex items-center gap-2">
          <Input placeholder="Tìm tên / SKU..." value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && (setPage(1), setKeyword(search))} className="w-56" />
          <MohoSeedButton onDone={load} />
          <SyncButton resource="product" onDone={load} />
          <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" /> Thêm mới</Button>
        </div>
      </div>

      <DataTable columns={columns} data={data} rowKey={(p) => p.id} loading={loading} page={page} pageSize={pageSize} total={total} onPage={setPage} />

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-h-[88vh] overflow-y-auto sm:max-w-3xl">
          <DialogHeader>
            <DialogTitle>{editingId ? 'Sửa sản phẩm' : 'Thêm sản phẩm'}</DialogTitle>
          </DialogHeader>

          <Tabs defaultValue="info">
            <TabsList>
              <TabsTrigger value="info">Thông tin</TabsTrigger>
              <TabsTrigger value="variants" disabled={!editingId}>Biến thể</TabsTrigger>
              <TabsTrigger value="images" disabled={!editingId}>Ảnh</TabsTrigger>
            </TabsList>

            <TabsContent value="info" className="space-y-4 pt-2">
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div className="space-y-1.5">
                  <Label>Danh mục</Label>
                  <Select value={form.categoryId ? String(form.categoryId) : undefined} onValueChange={(v) => setForm({ ...form, categoryId: v })}>
                    <SelectTrigger><SelectValue placeholder="-- Chọn danh mục --" /></SelectTrigger>
                    <SelectContent>{cats.map((c) => <SelectItem key={c.id} value={c.id}>{c.name}</SelectItem>)}</SelectContent>
                  </Select>
                </div>
                <div className="space-y-1.5">
                  <Label>SKU</Label>
                  <Input value={String(form.sku ?? '')} onChange={(e) => setForm({ ...form, sku: e.target.value })} />
                </div>
                <div className="space-y-1.5 sm:col-span-2">
                  <Label>Tên sản phẩm</Label>
                  <Input value={String(form.name ?? '')} onChange={(e) => setForm({ ...form, name: e.target.value })} />
                </div>
                <div className="space-y-1.5">
                  <Label>Phong cách</Label>
                  <Input value={String(form.style ?? '')} onChange={(e) => setForm({ ...form, style: e.target.value })} />
                </div>
                <div className="space-y-1.5">
                  <Label>Slug (để trống tự sinh)</Label>
                  <Input value={String(form.slug ?? '')} onChange={(e) => setForm({ ...form, slug: e.target.value })} />
                </div>
                <div className="space-y-1.5 sm:col-span-2">
                  <Label>Mô tả ngắn</Label>
                  <Textarea rows={2} value={String(form.shortDescription ?? '')} onChange={(e) => setForm({ ...form, shortDescription: e.target.value })} />
                </div>
                <div className="space-y-1.5 sm:col-span-2">
                  <Label>Mô tả chi tiết</Label>
                  <RichTextEditor value={String(form.longDescription ?? '')} onChange={(v) => setForm({ ...form, longDescription: v })} />
                </div>
              </div>
              <Button onClick={saveProduct} disabled={saving}>{saving ? 'Đang lưu...' : 'Lưu sản phẩm'}</Button>
            </TabsContent>

            <TabsContent value="variants" className="space-y-4 pt-2">
              <div className="grid grid-cols-2 gap-3 rounded-lg border p-3 sm:grid-cols-4">
                <div className="space-y-1"><Label>SKU</Label><Input value={String(vForm.skuVariant ?? '')} onChange={(e) => setVForm({ ...vForm, skuVariant: e.target.value })} /></div>
                <div className="space-y-1"><Label>Kích thước</Label><Input value={String(vForm.size ?? '')} onChange={(e) => setVForm({ ...vForm, size: e.target.value })} /></div>
                <div className="space-y-1"><Label>Chất liệu</Label><Input value={String(vForm.material ?? '')} onChange={(e) => setVForm({ ...vForm, material: e.target.value })} /></div>
                <div className="space-y-1"><Label>Màu</Label><Input value={String(vForm.color ?? '')} onChange={(e) => setVForm({ ...vForm, color: e.target.value })} /></div>
                <div className="space-y-1">
                  <Label>Tình trạng</Label>
                  <Select value={String(vForm.condition ?? ProductCondition.New)} onValueChange={(v) => setVForm({ ...vForm, condition: Number(v) })}>
                    <SelectTrigger><SelectValue /></SelectTrigger>
                    <SelectContent>{enumOptions(conditionLabel).map((o) => <SelectItem key={o.value} value={String(o.value)}>{o.label}</SelectItem>)}</SelectContent>
                  </Select>
                </div>
                <div className="space-y-1"><Label>Giá</Label><Input type="number" value={Number(vForm.price ?? 0)} onChange={(e) => setVForm({ ...vForm, price: Number(e.target.value) })} /></div>
                <div className="space-y-1"><Label>Tồn kho</Label><Input type="number" value={Number(vForm.stockQuantity ?? 0)} onChange={(e) => setVForm({ ...vForm, stockQuantity: Number(e.target.value) })} /></div>
                <div className="flex items-end"><Button className="w-full" onClick={saveVariant}>{vEditId ? 'Cập nhật' : 'Thêm'}</Button></div>
              </div>
              <Table>
                <TableHeader><TableRow><TableHead>SKU</TableHead><TableHead>Thuộc tính</TableHead><TableHead>Tình trạng</TableHead><TableHead className="text-right">Giá</TableHead><TableHead className="text-right">Tồn</TableHead><TableHead></TableHead></TableRow></TableHeader>
                <TableBody>
                  {variants.length === 0 ? (
                    <TableRow><TableCell colSpan={6} className="text-center text-muted-foreground">Chưa có biến thể</TableCell></TableRow>
                  ) : variants.map((v) => (
                    <TableRow key={v.id}>
                      <TableCell>{v.skuVariant}</TableCell>
                      <TableCell className="text-muted-foreground">{[v.size, v.material, v.color].filter(Boolean).join(' / ')}</TableCell>
                      <TableCell>{conditionLabel[v.condition]}</TableCell>
                      <TableCell className="text-right">{currency(v.price)}</TableCell>
                      <TableCell className="text-right">{v.stockQuantity}</TableCell>
                      <TableCell className="text-right">
                        <Button variant="ghost" size="icon" onClick={() => { setVEditId(v.id); setVForm({ skuVariant: v.skuVariant, size: v.size, material: v.material, color: v.color, condition: v.condition, price: v.price, stockQuantity: v.stockQuantity }) }}><Pencil className="h-4 w-4" /></Button>
                        <Button variant="ghost" size="icon" className="text-red-600" onClick={() => delVariant(v)}><Trash2 className="h-4 w-4" /></Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TabsContent>

            <TabsContent value="images" className="space-y-3 pt-2">
              <Button variant="outline" onClick={() => setPicker(true)}><Plus className="mr-2 h-4 w-4" /> Thêm ảnh từ kho</Button>
              <div className="grid grid-cols-3 gap-3 sm:grid-cols-5">
                {images.length === 0 && <p className="col-span-full text-sm text-muted-foreground">Chưa có ảnh</p>}
                {images.map((img) => (
                  <div key={img.id} className="group relative aspect-square overflow-hidden rounded-lg border">
                    <img src={mediaUrl(img.imageUrl)} className="h-full w-full object-cover" />
                    {img.isPrimary && <span className="absolute left-1 top-1"><Star className="h-4 w-4 fill-amber-400 text-amber-400" /></span>}
                    <Button variant="destructive" size="icon" className="absolute right-1 top-1 hidden h-6 w-6 group-hover:flex" onClick={() => delImage(img)}><Trash2 className="h-3 w-3" /></Button>
                  </div>
                ))}
              </div>
              <MediaPicker open={picker} onOpenChange={setPicker} onSelect={addImage} />
            </TabsContent>
          </Tabs>

          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>Đóng</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
