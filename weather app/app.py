# app.py - Main Flask Application
from flask import Flask, send_file
from flask_cors import CORS
from routes import weather_bp
from config import Config
import os

# Get the directory where this script is located
BASE_DIR = os.path.dirname(os.path.abspath(__file__))

def create_app():
    app = Flask(__name__, static_folder=BASE_DIR, static_url_path='')
    app.config.from_object(Config)
    
    # Enable CORS
    CORS(app, resources={r"/api/*": {"origins": "*"}})
    
    # Register blueprints
    app.register_blueprint(weather_bp, url_prefix='/api')
    
    # Serve the frontend
    @app.route('/')
    def index():
        return send_file(os.path.join(BASE_DIR, 'index.html'))
    
    @app.route('/style.css')
    def style():
        return send_file(os.path.join(BASE_DIR, 'style.css'))
    
    @app.route('/script.js')
    def script():
        return send_file(os.path.join(BASE_DIR, 'script.js'))
    
    @app.route('/images/<path:filename>')
    def serve_images(filename):
        return send_file(os.path.join(BASE_DIR, 'images', filename))
    
    @app.route('/<path:path>')
    def serve_static(path):
        full_path = os.path.join(BASE_DIR, path)
        if os.path.exists(full_path):
            return send_file(full_path)
        return "File not found", 404
    
    return app

if __name__ == '__main__':
    app = create_app()
    print("\n🚀 Starting Weather App...")
    print("📍 Open your browser to: http://localhost:8000\n")
    app.run(debug=False, host='0.0.0.0', port=8000, threaded=True)
