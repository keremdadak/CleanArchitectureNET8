using App.Repositories;
using App.Repositories.Products;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products
{
    public class ProductService(IProductRepository productRepository,IUnitOfWork unitOfWork) : IProductService
    {

        public async Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductAsync(int count)
        {
            var products = await productRepository.GetTopPriceProductsAsync(count);
            var productAsDto=products.Select(x=> new ProductResponse(x.Id,x.Name,x.Price,x.Stock)).ToList();
            return new ServiceResult<List<ProductResponse>>()
            {
                Data = productAsDto
            };
        }

        public async Task<ServiceResult<List<ProductResponse>>> GetAllListAsync()
        {
            var products = await productRepository.GetAll().ToListAsync();
            var productAsDto=products.Select(x=> new ProductResponse(x.Id,x.Name,x.Price,x.Stock)).ToList();

            return ServiceResult<List<ProductResponse>>.Success(productAsDto);
        }
        public async Task<ServiceResult<ProductResponse>> GetByIdAsync(int id)
        {

            var product = await productRepository.GetByIdAsync(id);
            if(product is null)
            {
                ServiceResult<Product>.Fail("Product not found", HttpStatusCode.NotFound);
            }
            var productAsDto = new ProductResponse(product!.Id, product.Name, product.Price, product.Stock);
            return ServiceResult<ProductResponse>.Success(productAsDto)!;
            
        }
        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            var product = new Product()
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock,
            };
            await productRepository.AddAsync(product);
            await unitOfWork.SaveChangeAsync();
            return ServiceResult<CreateProductResponse>.Success(new CreateProductResponse(product.Id));
        }
        public async Task<ServiceResult> UpdateAsync(int id,UpdateProductRequest request)
        {
            var product =await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }
            product.Name = request.Name; 
            product.Price= request.Price; 
            product.Stock = request.Stock;

            productRepository.Update(product);
            await unitOfWork.SaveChangeAsync();

            return ServiceResult.Success();
        }
        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if(product is null)
            {
                ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }
            productRepository.Delete(product);
            await unitOfWork.SaveChangeAsync();

            return ServiceResult.Success();
        }
    }
}
