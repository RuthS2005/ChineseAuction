import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient ,withInterceptors } from '@angular/common/http'; // <--- ייבוא חשוב
import { tokenInterceptor } from './services/token'; // ייבוא האינטרספטור
import { provideAnimations } from '@angular/platform-browser/animations'; // <--- ייבוא חשוב

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([tokenInterceptor])),
    provideAnimations() // <--- הוספה כאן
    
    ]
};
 