import {ArticlesPanel} from 'sun'
import {ArticlesMultiCatPage} from 'sun'
import {Categories1} from 'sun'
import {Material} from 'sun'
import {ArticlesPage} from 'sun'

export default {
  name: 'Articles1',
  title: 'Articles 1',
  categoryType: 'Articles',

  setCategoryRoute(category) {
    category.route = {
      name: `articles-${category.name}`,
      params: {}
    };

    for (const cat of category.subCategories) {
      cat.route = {
        name: `articles-${category.name}-cat`,
        params: {
          categoryName: cat.name
        }
      }
    }
  },

  getRoutes(category) {
    const name = category.name;
    const nameLower = name.toLowerCase();

    return [
      {
        name: `articles-${name}`,
        path: '/' + nameLower,
        components: {
          default: ArticlesMultiCatPage,
          navigation: ArticlesPanel
        },
        props: {
          default: {categoriesNames: nameLower, pageTitle: category.title},
          navigation: {categories: Categories1, categoryName: name}
        },
      },
      {
        name: `articles-${name}-cat`,
        path: `/${nameLower}/:categoryName`,
        components: {
          default: ArticlesPage,
          navigation: ArticlesPanel
        },
        props: {
          default: true,
          navigation: {categories: Categories1, categoryName: name}
        }
      },
      {
        name: `articles-${name}-cat-mat`,
        path: `/${nameLower}/:categoryName/:idOrName`,
        components: {
          default: Material,
          navigation: ArticlesPanel
        },
        props: {
          default: (route) => {
            return {categoryName: route.params.categoryName, idOrName: route.params.idOrName}
          },
          navigation: {categories: Categories1, categoryName: name}
        }
      }
    ]
  }
}
