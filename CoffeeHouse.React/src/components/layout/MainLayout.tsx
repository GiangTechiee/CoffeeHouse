import React from 'react';
import Navbar from './Navbar';
import Footer from './Footer';

interface MainLayoutProps {
    children: React.ReactNode;
}

const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
    return (
        <div className="min-h-screen flex flex-col grain relative">
            <Navbar />
            <main className="flex-grow pt-24 md:pt-32">
                {children}
            </main>
            <Footer />
        </div>
    );
};

export default MainLayout;
