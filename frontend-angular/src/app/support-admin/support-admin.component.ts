import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { inject } from '@angular/core';

@Component({
  selector: 'app-support-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './support-admin.component.html',
  styleUrls: ['./support-admin.component.css']
})
export class SupportAdminComponent {
  title: string = '';
  description: string = '';
  message: string = '';

  private http = inject(HttpClient);

  addSupportTopic() {
    if (!this.title.trim() || !this.description.trim()) {
      this.message = 'Please enter both title and description.';
      return;
    }

    const payload = {
      title: this.title,
      description: this.description
    };

    // Expect API to return an object with `message` and `id`
    this.http.post<{ message: string; id: number }>('http://localhost:5140/api/SupportAdmin/add', payload)
      .subscribe({
        next: (response) => {
          console.log('API Response:', response); // Debugging
          this.message = `✅ ${response.message} (ID: ${response.id})`;
          this.title = '';
          this.description = '';
        },
        error: (error) => {
          console.error('Error:', error);
          this.message = error.error.message || '❌ Error adding support topic.';
        }
      });
  }
}
