import { useEffect, useRef } from 'react'
import { useEditor, EditorContent } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Underline from '@tiptap/extension-underline'
import Link from '@tiptap/extension-link'
import Image from '@tiptap/extension-image'
import TextAlign from '@tiptap/extension-text-align'
import Placeholder from '@tiptap/extension-placeholder'
import {
  Bold,
  Italic,
  Underline as UnderlineIcon,
  Strikethrough,
  Heading2,
  Heading3,
  List,
  ListOrdered,
  Quote,
  AlignLeft,
  AlignCenter,
  AlignRight,
  Link as LinkIcon,
  Image as ImageIcon,
  Undo,
  Redo,
} from 'lucide-react'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import { Separator } from '@/components/ui/separator'
import { mediaApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from './Toast'
import { mediaUrl } from '@/lib/format'

interface Props {
  value?: string
  onChange?: (html: string) => void
  placeholder?: string
}

export default function RichTextEditor({ value, onChange, placeholder = 'Nhập nội dung...' }: Props) {
  const toast = useToast()
  const fileRef = useRef<HTMLInputElement>(null)

  const editor = useEditor({
    extensions: [
      StarterKit,
      Underline,
      Link.configure({ openOnClick: false }),
      Image,
      TextAlign.configure({ types: ['heading', 'paragraph'] }),
      Placeholder.configure({ placeholder }),
    ],
    content: value || '',
    onUpdate: ({ editor }) => onChange?.(editor.getHTML()),
    editorProps: { attributes: { class: 'tiptap' } },
  })

  useEffect(() => {
    if (editor && value !== undefined && value !== editor.getHTML()) {
      editor.commands.setContent(value || '', false)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value, editor])

  if (!editor) return null

  const setLink = () => {
    const prev = editor.getAttributes('link').href as string | undefined
    const url = window.prompt('Nhập đường dẫn liên kết:', prev ?? 'https://')
    if (url === null) return
    if (url === '') editor.chain().focus().extendMarkRange('link').unsetLink().run()
    else editor.chain().focus().extendMarkRange('link').setLink({ href: url }).run()
  }

  const uploadImage = async (file: File) => {
    try {
      const media = await mediaApi.upload(file, 'posts')
      editor.chain().focus().setImage({ src: mediaUrl(media.url) }).run()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  return (
    <div className="rounded-lg border">
      <div className="flex flex-wrap items-center gap-0.5 border-b bg-muted/40 p-1.5">
        <Tb on={editor.isActive('bold')} onClick={() => editor.chain().focus().toggleBold().run()}><Bold className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive('italic')} onClick={() => editor.chain().focus().toggleItalic().run()}><Italic className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive('underline')} onClick={() => editor.chain().focus().toggleUnderline().run()}><UnderlineIcon className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive('strike')} onClick={() => editor.chain().focus().toggleStrike().run()}><Strikethrough className="h-4 w-4" /></Tb>
        <Sep />
        <Tb on={editor.isActive('heading', { level: 2 })} onClick={() => editor.chain().focus().toggleHeading({ level: 2 }).run()}><Heading2 className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive('heading', { level: 3 })} onClick={() => editor.chain().focus().toggleHeading({ level: 3 }).run()}><Heading3 className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive('bulletList')} onClick={() => editor.chain().focus().toggleBulletList().run()}><List className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive('orderedList')} onClick={() => editor.chain().focus().toggleOrderedList().run()}><ListOrdered className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive('blockquote')} onClick={() => editor.chain().focus().toggleBlockquote().run()}><Quote className="h-4 w-4" /></Tb>
        <Sep />
        <Tb on={editor.isActive({ textAlign: 'left' })} onClick={() => editor.chain().focus().setTextAlign('left').run()}><AlignLeft className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive({ textAlign: 'center' })} onClick={() => editor.chain().focus().setTextAlign('center').run()}><AlignCenter className="h-4 w-4" /></Tb>
        <Tb on={editor.isActive({ textAlign: 'right' })} onClick={() => editor.chain().focus().setTextAlign('right').run()}><AlignRight className="h-4 w-4" /></Tb>
        <Sep />
        <Tb on={editor.isActive('link')} onClick={setLink}><LinkIcon className="h-4 w-4" /></Tb>
        <Tb on={false} onClick={() => fileRef.current?.click()}><ImageIcon className="h-4 w-4" /></Tb>
        <Sep />
        <Tb on={false} onClick={() => editor.chain().focus().undo().run()}><Undo className="h-4 w-4" /></Tb>
        <Tb on={false} onClick={() => editor.chain().focus().redo().run()}><Redo className="h-4 w-4" /></Tb>
        <input
          ref={fileRef}
          type="file"
          accept="image/*"
          className="hidden"
          onChange={(e) => e.target.files?.[0] && uploadImage(e.target.files[0])}
        />
      </div>
      <EditorContent editor={editor} />
    </div>
  )
}

function Tb({ on, onClick, children }: { on: boolean; onClick: () => void; children: React.ReactNode }) {
  return (
    <Button
      type="button"
      variant={on ? 'secondary' : 'ghost'}
      size="icon"
      className={cn('h-8 w-8', on && 'text-primary')}
      onClick={onClick}
    >
      {children}
    </Button>
  )
}

function Sep() {
  return <Separator orientation="vertical" className="mx-1 h-5" />
}
