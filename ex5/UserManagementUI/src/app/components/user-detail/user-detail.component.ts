import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { User } from '../../models/user.model';

/**
 * Presentation component for displaying user details
 * Emits events for edit and delete actions to parent component
 * Read-only view of user information
 */
@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent {
  @Input() user!: User;
  @Output() editUser = new EventEmitter<User>();
  @Output() deleteUser = new EventEmitter<User>();

  /**
   * Emits edit event to parent component
   */
  onEdit(): void {
    this.editUser.emit(this.user);
  }

  /**
   * Emits delete event to parent component
   */
  onDelete(): void {
    this.deleteUser.emit(this.user);
  }

  /**
   * Opens website in new tab
   * Handles both URLs with and without protocol
   */
  openWebsite(): void {
    if (!this.user.website) return;

    let url = this.user.website;
    // Add http:// if no protocol specified
    if (!url.match(/^https?:\/\//)) {
      url = 'http://' + url;
    }

    window.open(url, '_blank', 'noopener,noreferrer');
  }

  /**
   * Formats date for display
   */
  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
