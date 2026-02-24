import { motion } from 'framer-motion';
import { ArrowRight } from 'lucide-react';
import { Link } from 'react-router-dom';

const Hero = () => {
    return (
        <section className="relative min-h-[80vh] flex items-center px-6 md:px-12 py-20 overflow-hidden">
            <div className="max-w-7xl mx-auto w-full grid grid-cols-1 lg:grid-cols-2 gap-20 items-center">

                {/* Text Content */}
                <div className="relative z-10 order-2 lg:order-1">
                    <motion.div
                        initial={{ opacity: 0, y: 30 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.8, ease: [0.16, 1, 0.3, 1] }}
                    >
                        <span className="text-xs uppercase tracking-[0.4em] text-coffee-600 font-semibold mb-6 block">
                            Established 2024
                        </span>
                        <h1 className="text-6xl md:text-8xl font-display font-medium leading-[1] text-coffee-800 mb-8">
                            Khởi đầu <br />
                            <span className="italic font-light">sự tinh tế</span>
                        </h1>
                        <p className="text-coffee-600 text-lg md:text-xl font-light leading-relaxed max-w-md mb-12">
                            Khám phá hương vị cà phê nguyên bản trong không gian tối giản và ấm cúng, nơi mỗi hạt cà phê là một câu chuyện.
                        </p>

                        <div className="flex flex-col sm:flex-row gap-6">
                            <Link
                                to="/products"
                                className="btn-primary flex items-center justify-center gap-3 group"
                            >
                                Khám phá thực đơn
                                <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
                            </Link>
                            <Link
                                to="/about"
                                className="btn-outline flex items-center justify-center"
                            >
                                Câu chuyện
                            </Link>
                        </div>
                    </motion.div>
                </div>

                {/* Image Content */}
                <div className="relative order-1 lg:order-2">
                    <motion.div
                        initial={{ scale: 1.1, opacity: 0 }}
                        animate={{ scale: 1, opacity: 1 }}
                        transition={{ duration: 1.2, ease: [0.16, 1, 0.3, 1] }}
                        className="aspect-[4/5] relative"
                    >
                        <div className="absolute inset-0 bg-coffee-200 translate-x-4 translate-y-4" />
                        <img
                            src="https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?q=80&w=2070&auto=format&fit=crop"
                            alt="Premium Coffee"
                            className="w-full h-full object-cover relative z-10 grayscale hover:grayscale-0 transition-all duration-1000 grayscale-[0.5]"
                        />

                        {/* Signature Floating Card */}
                        <motion.div
                            initial={{ x: 50, opacity: 0 }}
                            animate={{ x: 0, opacity: 1 }}
                            transition={{ delay: 0.8, duration: 0.8 }}
                            className="absolute -bottom-10 -left-10 bg-white p-8 shadow-2xl z-20 hidden md:block"
                        >
                            <p className="font-display text-2xl italic text-coffee-800">Signature Blend</p>
                            <p className="text-[10px] uppercase tracking-widest text-coffee-400 mt-2">100% Arabica Cầu Đất</p>
                        </motion.div>
                    </motion.div>
                </div>
            </div>

            {/* Background Decorative Elements */}
            <div className="absolute top-0 right-0 w-1/3 h-full bg-coffee-100/30 -z-10" />
            <div className="absolute top-1/4 left-10 text-[20rem] font-display font-bold text-coffee-100/10 select-none -z-10">
                01
            </div>
        </section>
    );
};

export default Hero;
