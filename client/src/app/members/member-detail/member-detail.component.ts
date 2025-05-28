import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule, TimeagoPipe } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';
import { Visit } from '../../_models/visit';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberMessagesComponent, CommonModule]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  private messageService = inject(MessageService);
  public accountService = inject(AccountService);
  presenceService = inject(PresenceService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  memberService = inject(MembersService);
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  visits: Visit[] = [];


  ngOnInit(): void {

    
    const user = this.accountService.currentUser();

    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member?.photos.map(p => {
          this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
        });

        console.log('Is VIP:', user?.roles?.includes('VIP'));
console.log('Current user:', user?.username);
console.log('Viewing member:', this.member.username);

        if (user && user.roles?.includes('VIP') && user.username !== this.member.username) {
          this.memberService.trackProfileVisit(this.member.username).subscribe({
            next: () => console.log(`Visit tracked: ${user.username} -> ${this.member.username}`),
            error: err => console.error('Error tracking visit', err)
          });
        }
      }
    });

    this.route.paramMap.subscribe({
      next: _ => this.onRouteParamsChange()
    });

    this.route.queryParams.subscribe({
      next: params => {
        if (params['tab']) {
          setTimeout(() => {
            this.selectTab(params['tab']);
          }, 0);
        }
      }
    });
  }



  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find(x => x.heading === heading);
      if (messageTab) messageTab.active = true;
    }
  }

  onRouteParamsChange() {
    const user = this.accountService.currentUser();
    if (!user) return
    if (this.messageService.hubConnection?.state === HubConnectionState.Connected && this.activeTab?.heading === 'Messages') {
      this.messageService.hubConnection.stop().then(() => {
        this.messageService.createHubConnection(user, this.member.username);
      })
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: this.activeTab.heading },
      queryParamsHandling: 'merge'
    });

    const user = this.accountService.currentUser();
    if (!user || !this.member) return;

    if (this.activeTab.heading === 'Messages') {
      if (this.messageService.hubConnection?.state !== HubConnectionState.Connected) {
        this.messageService.createHubConnection(user, this.member.username);
      }
    } else {
      this.messageService.stopHubConnection();
    }

    const currentUser = this.accountService.currentUser();
    if (currentUser && currentUser.roles.includes('VIP')) {
      this.memberService.trackProfileVisit(this.member.username).subscribe();
    }

    if (this.activeTab.heading === 'Visitors') {
      this.loadVisitHistory();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  loadVisitHistory() {
    this.memberService.getVisitHistory().subscribe({
      next: visits => this.visits = visits,
      error: err => console.error('Failed to load visit history', err)
    });
  }
}