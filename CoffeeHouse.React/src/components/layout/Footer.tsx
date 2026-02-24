import { Coffee, Instagram, Facebook, Twitter, Phone, Mail, MapPin } from 'lucide-react';

const Footer = () => {
    return (
        <footer className="bg-coffee-900 text-coffee-100 py-20 px-6 md:px-12">
            <div className="max-w-7xl mx-auto grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-12 lg:gap-8">

                {/* Brand */}
                <div className="space-y-6">
                    <div className="flex items-center gap-3">
                        <div className="bg-coffee-300 p-2">
                            <Coffee className="w-6 h-6 text-coffee-900" />
                        </div>
                        <span className="text-2xl font-display font-bold tracking-tighter">COFFEE HOUSE</span>
                    </div>
                    <p className="text-coffee-300/80 leading-relaxed font-light text-sm">
                        Nơi kết nối đam mê và những hương vị cà phê thượng hạng nhất. Chúng tôi mang đến trải nghiệm cà phê đích thực mỗi ngày.
                    </p>
                    <div className="flex gap-4">
                        <a href="#" className="w-10 h-10 border border-coffee-700 flex items-center justify-center hover:bg-coffee-300 hover:text-coffee-900 transition-all duration-500">
                            <Instagram className="w-4 h-4" />
                        </a>
                        <a href="#" className="w-10 h-10 border border-coffee-700 flex items-center justify-center hover:bg-coffee-300 hover:text-coffee-900 transition-all duration-500">
                            <Facebook className="w-4 h-4" />
                        </a>
                        <a href="#" className="w-10 h-10 border border-coffee-700 flex items-center justify-center hover:bg-coffee-300 hover:text-coffee-900 transition-all duration-500">
                            <Twitter className="w-4 h-4" />
                        </a>
                    </div>
                </div>

                {/* Quick Links */}
                <div>
                    <h4 className="font-display text-lg mb-8 uppercase tracking-widest text-coffee-300">Khám Phá</h4>
                    <ul className="space-y-4 text-sm font-light">
                        <li><a href="/" className="hover:text-coffee-300 transition-colors">Trang chủ</a></li>
                        <li><a href="/products" className="hover:text-coffee-300 transition-colors">Thực đơn</a></li>
                        <li><a href="/news" className="hover:text-coffee-300 transition-colors">Chuyện cà phê</a></li>
                        <li><a href="/about" className="hover:text-coffee-300 transition-colors">Về chúng tôi</a></li>
                    </ul>
                </div>

                {/* Support */}
                <div>
                    <h4 className="font-display text-lg mb-8 uppercase tracking-widest text-coffee-300">Hỗ Trợ</h4>
                    <ul className="space-y-4 text-sm font-light">
                        <li><a href="#" className="hover:text-coffee-300 transition-colors">Câu hỏi thường gặp</a></li>
                        <li><a href="#" className="hover:text-coffee-300 transition-colors">Chính sách bảo mật</a></li>
                        <li><a href="#" className="hover:text-coffee-300 transition-colors">Điều khoản dịch vụ</a></li>
                        <li><a href="#" className="hover:text-coffee-300 transition-colors">Liên hệ</a></li>
                    </ul>
                </div>

                {/* Contact */}
                <div>
                    <h4 className="font-display text-lg mb-8 uppercase tracking-widest text-coffee-300">Liên Hệ</h4>
                    <ul className="space-y-6 text-sm font-light">
                        <li className="flex gap-4 items-start">
                            <MapPin className="w-5 h-5 text-coffee-300 shrink-0" />
                            <span className="text-coffee-300/80">123 Đường Cà Phê, Quận 1, TP. Hồ Chí Minh</span>
                        </li>
                        <li className="flex gap-4 items-center">
                            <Phone className="w-5 h-5 text-coffee-300 shrink-0" />
                            <span className="text-coffee-300/80">0123 456 789</span>
                        </li>
                        <li className="flex gap-4 items-center">
                            <Mail className="w-5 h-5 text-coffee-300 shrink-0" />
                            <span className="text-coffee-300/80">hello@coffeehouse.vn</span>
                        </li>
                    </ul>
                </div>
            </div>

            <div className="max-w-7xl mx-auto border-t border-coffee-800 mt-20 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px] uppercase tracking-[0.2em] text-coffee-600">
                <p>© 2024 COFFEE HOUSE. ALL RIGHTS RESERVED.</p>
                <p>DESIGNED WITH PASSION BY AGENT</p>
            </div>
        </footer>
    );
};

export default Footer;
