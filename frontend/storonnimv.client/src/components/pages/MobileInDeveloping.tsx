import { FC } from "react";

const MobileInDeveloping: FC = () => {
    return (
        <div style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            height: "100vh",
            background: "linear-gradient(135deg, #1e1e2f, #2c3e50)",
            color: "white",
            padding: "20px",
            textAlign: "center"
        }}>
            <div style={{
                maxWidth: "400px",
                animation: "fadeIn 0.8s ease-in-out",
                boxShadow: "0 4px 20px rgba(0,0,0,0.4)",
                backgroundColor: "rgba(255, 255, 255, 0.05)",
                borderRadius: "16px",
                padding: "24px",
                backdropFilter: "blur(6px)"
            }}>
                <h2 style={{
                    fontSize: "1.5rem",
                    marginBottom: "16px",
                    fontWeight: 600,
                    color: "#ffcc70"
                }}>
                    В розробці
                </h2>
                <p style={{
                    fontSize: "1rem",
                    lineHeight: "1.5"
                }}>
                    Мобільна та планшетна версія зараз у розробці.<br />
                    Будь ласка, зайдіть через ноутбук чи комп'ютер.
                </p>
            </div>
            <style>
                {`
                @keyframes fadeIn {
                    from { opacity: 0; transform: translateY(10px); }
                    to { opacity: 1; transform: translateY(0); }
                }
                `}
            </style>
        </div>
    );
};

export { MobileInDeveloping };
