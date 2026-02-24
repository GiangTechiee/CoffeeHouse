import Hero from '../components/home/Hero';
import CategoryStrip from '../components/home/CategoryStrip';
import FeaturedProducts from '../components/home/FeaturedProducts';
import { motion } from 'framer-motion';

const Home = () => {
    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.5 }}
        >
            <Hero />
            <CategoryStrip />
            <FeaturedProducts />

            {/* Editorial Section */}
            <section className="py-24 px-6 md:px-12 bg-white overflow-hidden">
                <div className="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-2 gap-20 items-center">
                    <div className="relative">
                        <motion.div
                            initial={{ x: -100, opacity: 0 }}
                            whileInView={{ x: 0, opacity: 1 }}
                            viewport={{ once: true }}
                            transition={{ duration: 1 }}
                            className="aspect-[16/9] bg-coffee-100 overflow-hidden"
                        >
                            <img
                                src="https://images.unsplash.com/photo-1442154321678-201ad74c2361?q=80&w=2070&auto=format&fit=crop"
                                alt="Coffee Roasting"
                                className="w-full h-full object-cover grayscale hover:grayscale-0 transition-all duration-1000"
                            />
                        </motion.div>
                        <div className="absolute -bottom-10 -right-10 w-2/3 aspect-video bg-coffee-800 text-white p-10 flex flex-col justify-end">
                            <h3 className="text-3xl font-display mb-4">Nghệ thuật rang say</h3>
                            <p className="text-sm text-coffee-300 font-light leading-relaxed">
                                Chúng tôi tin rằng mỗi hạt cà phê đều mang trong mình một tâm hồn. Quá trình rang xay được thực hiện tỉ mỉ để đánh thức những nốt hương tinh túy nhất.
                            </p>
                        </div>
                    </div>

                    <div className="space-y-10 lg:pl-10">
                        <span className="text-[10px] uppercase tracking-[0.4em] text-coffee-400 font-bold">Kỹ nghệ</span>
                        <h2 className="text-5xl md:text-6xl font-display text-coffee-800 leading-[1.1]">
                            Từ nông trại <br />đến tách <span className="italic">cà phê</span>
                        </h2>
                        <div className="space-y-6 text-coffee-600 font-light leading-relaxed max-w-md">
                            <p>
                                Hành trình của chúng tôi bắt đầu từ những vùng cao nguyên trù phú, nơi những trái cà phê chín mọng được thu hoạch thủ công.
                            </p>
                            <p>
                                Mỗi mẻ rang là một bản giao hưởng của nhiệt độ và thời gian, được điều khiển bởi những chuyên gia tâm huyết.
                            </p>
                        </div>
                        <button className="btn-outline">
                            Khám phá quy trình
                        </button>
                    </div>
                </div>
            </section>
        </motion.div>
    );
};

export default Home;
