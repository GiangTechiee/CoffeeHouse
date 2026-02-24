import { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import api from '../../services/api';
import type { ApiResponse, PaginatedResponse, Category } from '../../types';

const iconMap: Record<string, string> = {
    'Cà phê': '☕',
    'Trà trái cây': '🍹',
    'Đá xay': '🥤',
    'Bánh & Snack': '🥐',
    'Tại nhà': '🏠',
    'Trà sữa': '🧋',
    'Đặc sản': '⭐',
    'Default': '☕'
};

const CategoryStrip = () => {
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await api.get<ApiResponse<PaginatedResponse<Category>>>('/v1/categories');
                if (response.data.success) {
                    setCategories(response.data.data.items);
                }
            } catch (error) {
                console.error('Failed to fetch categories:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchCategories();
    }, []);

    if (loading) {
        return (
            <section className="bg-white border-y border-coffee-100 py-12">
                <div className="max-w-7xl mx-auto px-6">
                    <div className="flex flex-wrap justify-center md:justify-between items-center gap-8">
                        {[1, 2, 3, 4, 5].map((i) => (
                            <div key={i} className="flex flex-col items-center gap-3 animate-pulse">
                                <div className="w-16 h-16 rounded-full bg-coffee-50"></div>
                                <div className="h-2 bg-coffee-50 w-12"></div>
                            </div>
                        ))}
                    </div>
                </div>
            </section>
        );
    }

    return (
        <section className="bg-white border-y border-coffee-100 py-12">
            <div className="max-w-7xl mx-auto px-6">
                <div className="flex flex-wrap justify-center md:justify-between items-center gap-8">
                    {categories.map((cat) => (
                        <motion.button
                            whileHover={{ y: -5 }}
                            key={cat.id}
                            className="flex flex-col items-center gap-3 group"
                        >
                            <div className="w-16 h-16 rounded-full bg-coffee-50 flex items-center justify-center text-2xl group-hover:bg-coffee-700 group-hover:text-white transition-all duration-500">
                                {iconMap[cat.name] || iconMap['Default']}
                            </div>
                            <span className="text-[10px] uppercase tracking-[0.2em] font-medium text-coffee-600 group-hover:text-coffee-900 transition-colors">
                                {cat.name}
                            </span>
                        </motion.button>
                    ))}
                </div>
            </div>
        </section>
    );
};

export default CategoryStrip;
