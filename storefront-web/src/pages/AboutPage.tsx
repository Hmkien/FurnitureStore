import { Link } from 'react-router-dom'
import { Leaf, Hammer, HeartHandshake, Sparkles, ArrowRight } from 'lucide-react'

const pillars = [
  { icon: Leaf, title: 'Bền vững', desc: 'Gỗ tự nhiên khai thác có trách nhiệm, hoàn thiện bằng sơn gốc nước an toàn.' },
  { icon: Hammer, title: 'Thủ công', desc: 'Mỗi sản phẩm được hoàn thiện tỉ mỉ bởi những người thợ lành nghề.' },
  { icon: HeartHandshake, title: 'Tận tâm', desc: 'Tư vấn không gian, giao lắp tận nơi và bảo hành dài lâu.' },
  { icon: Sparkles, title: 'Tinh tế', desc: 'Đường nét tối giản, vượt thời gian, hợp mọi không gian sống.' },
]

export default function AboutPage() {
  return (
    <div className="pb-4">
      {/* Hero */}
      <section className="border-b border-line bg-paper">
        <div className="u-container grid gap-10 py-16 lg:grid-cols-2 lg:items-center">
          <div className="animate-fade-up">
            <span className="eyebrow">Về chúng tôi</span>
            <h1 className="display mt-4 text-4xl text-ink sm:text-5xl">
              Chúng tôi tin một ngôi nhà đẹp
              <br />
              bắt đầu từ <span className="italic text-clay-600">sự tử tế</span>
            </h1>
            <p className="mt-6 max-w-md leading-relaxed text-ink-soft">
              Furnitura ra đời với mong muốn mang nội thất thiết kế — vốn tinh tế và bền bỉ — đến gần hơn
              với mọi gia đình Việt. Chúng tôi chọn vật liệu tự nhiên, hợp tác cùng xưởng mộc lành nghề,
              và đặt trải nghiệm của bạn làm trọng tâm.
            </p>
            <Link to="/products" className="btn-primary mt-8">
              Khám phá sản phẩm <ArrowRight className="h-4 w-4" />
            </Link>
          </div>
          <div className="grid grid-cols-3 gap-4">
            <div className="col-span-2 aspect-[4/3] rounded-3xl bg-gradient-to-br from-clay-200 to-clay-400" />
            <div className="aspect-[3/4] rounded-3xl bg-ink/90" />
            <div className="aspect-[3/4] rounded-3xl bg-clay-100" />
            <div className="col-span-2 aspect-[4/3] rounded-3xl bg-gradient-to-tr from-ink to-clay-700" />
          </div>
        </div>
      </section>

      {/* Trụ cột giá trị */}
      <section className="u-container py-16">
        <div className="text-center">
          <span className="eyebrow">Giá trị cốt lõi</span>
          <h2 className="display mt-2 text-3xl text-ink sm:text-4xl">Điều làm nên Furnitura</h2>
        </div>
        <div className="mt-10 grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {pillars.map((p) => (
            <div key={p.title} className="surface p-6">
              <span className="grid h-12 w-12 place-items-center rounded-2xl bg-clay-50 text-clay-600">
                <p.icon className="h-6 w-6" />
              </span>
              <h3 className="mt-4 font-display text-xl text-ink">{p.title}</h3>
              <p className="mt-2 text-sm leading-relaxed text-ink-soft">{p.desc}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Số liệu */}
      <section className="u-container">
        <div className="grid grid-cols-2 gap-px overflow-hidden rounded-4xl border border-line bg-line sm:grid-cols-4">
          {[['2018', 'Năm thành lập'], ['12K+', 'Khách hàng'], ['500+', 'Mẫu thiết kế'], ['24th', 'Bảo hành']].map(([v, l]) => (
            <div key={l} className="bg-paper px-6 py-8 text-center">
              <div className="font-display text-3xl text-clay-600">{v}</div>
              <div className="mt-1 text-xs uppercase tracking-wide text-ink-muted">{l}</div>
            </div>
          ))}
        </div>
      </section>

      {/* CTA */}
      <section className="u-container mt-16">
        <div className="relative overflow-hidden rounded-4xl bg-ink px-8 py-16 text-center text-paper sm:px-16">
          <div className="absolute -right-16 -top-16 h-64 w-64 rounded-full bg-clay-600/30 blur-3xl" />
          <h2 className="display mx-auto max-w-xl text-3xl sm:text-4xl">Sẵn sàng làm mới không gian của bạn?</h2>
          <Link to="/products" className="btn mt-8 inline-flex bg-paper text-ink hover:bg-clay-50">
            Bắt đầu mua sắm <ArrowRight className="h-4 w-4" />
          </Link>
        </div>
      </section>
    </div>
  )
}
