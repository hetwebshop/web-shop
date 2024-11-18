import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-breadcrumbs',
  templateUrl: './breadcrumbs.component.html',
  styleUrls: ['./breadcrumbs.component.css']
})
export class BreadcrumbsComponent {
  // breadcrumbs: { label: string, url: string }[] = [];

  // constructor(
  //   private router: Router,
  //   private activatedRoute: ActivatedRoute
  // ) { }

  // ngOnInit(): void {
  //   console.log("breadcrums");
  //   console.log('ActivatedRoute:', this.activatedRoute);
  //   this.router.events
    
  // .pipe(
  //   tap(event => console.log(event)), // Add this line
  //   //filter(event => event instanceof NavigationEnd),
  //   map((event) =>{
  //     console.log(event)
  //     return  this.buildBreadcrumbs(this.activatedRoute.root);
  //   })
  // )
  // .subscribe(breadcrumbs => {
  //   this.breadcrumbs = breadcrumbs;
  // });
  // }

  // private buildBreadcrumbs(route: ActivatedRoute, url: string = '', breadcrumbs: { label: string, url: string }[] = []): { label: string, url: string }[] {
  //   console.log("from build");
  //   const routeSnapshot = route.snapshot;
  //   const label = routeSnapshot.data?.breadcrumb;
  //   const path = routeSnapshot.routeConfig?.path;

  //   const nextUrl = path ? `${url}/${path}` : url;

  //   const breadcrumb = label ? { label, url: nextUrl } : null;
  //   const newBreadcrumbs = breadcrumb ? [...breadcrumbs, breadcrumb] : [...breadcrumbs];
  //   console.log(newBreadcrumbs);
  //   return newBreadcrumbs;
  // }
}
