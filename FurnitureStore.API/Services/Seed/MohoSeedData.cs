namespace FurnitureStore.API.Services.Seed
{
    /// <summary>
    /// Dữ liệu danh mục + sản phẩm thật lấy từ moho.com.vn (tên, giá, ảnh) để seed demo.
    /// </summary>
    public static class MohoSeedData
    {
        public record SeedProduct(string Name, string Category, decimal Price, string ImageUrl);

        /// <summary>Danh mục con -> nhóm phòng (danh mục cha).</summary>
        public static readonly Dictionary<string, string> SubToRoom = new()
        {
            ["Ghế Sofa"] = "Phòng khách",
            ["Bàn Sofa/Trà"] = "Phòng khách",
            ["Tủ Kệ Tivi"] = "Phòng khách",
            ["Tủ Giày - Tủ Trang Trí"] = "Phòng khách",
            ["Giường Ngủ"] = "Phòng ngủ",
            ["Tủ Quần Áo"] = "Phòng ngủ",
            ["Tủ Đầu Giường"] = "Phòng ngủ",
            ["Bàn Trang Điểm"] = "Phòng ngủ",
            ["Bàn Ăn"] = "Phòng ăn",
            ["Ghế Ăn"] = "Phòng ăn",
            ["Bàn Làm Việc"] = "Phòng làm việc",
            ["Ghế Văn Phòng"] = "Phòng làm việc",
        };

        public static readonly SeedProduct[] Products =
        {
            new("Ghế Sofa 1m8 MOHO MOCHI", "Ghế Sofa", 9990000, "https://cdn.hstatic.net/products/200000065946/pro_be_ghe_sofa_mochi_noi_that_moho_7_11031ee2a5d84aeab448f2e050bdb119_grande.png"),
            new("Sofa Phòng Khách Hiện Đại KLINE", "Ghế Sofa", 10490000, "https://cdn.hstatic.net/products/200000065946/pro_xam_sofa_kline_noi_that_moho_27e5c0c8a64949b6a307228ca9cdfaa5_grande.png"),
            new("Ghế Sofa Moho Dalumd 301 (Màu Nâu 180)", "Ghế Sofa", 9490000, "https://product.hstatic.net/200000065946/product/pro_xam_noi_that_moho_sofa_1_e9ebd2d969ed4057a72a4aed1b288785_grande.jpg"),
            new("Ghế Sofa Góc Chữ L Gỗ Cao Su Tự Nhiên MOHO VLINE 601", "Ghế Sofa", 13990000, "https://product.hstatic.net/200000065946/product/pro_nau_pk_vline_be_2_47e4e6f485d24980bb731ca55e303493_grande.jpg"),
            new("Combo Sofa Gỗ Cao Su Chữ L MOHO HOBRO (Màu nâu, 2m7)", "Ghế Sofa", 19990000, "https://cdn.hstatic.net/products/200000065946/pro_combo_sofa_goc_phai_noi_that_moho_hobro_phong_khach_tay_ben_phai_434f1bba6220486bab3fce0facfce024_grande.png"),
            new("Ghế Sofa Gỗ Cao Su Tự Nhiên MOHO FYN 901", "Ghế Sofa", 10990000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_ghe_sofa_fyn_901_1b_c0de39fe45f545c98c68f075a1d9941d_grande.jpg"),

            new("Giường Ngủ Bọc Vải Phong Cách Ý MOHO COMET", "Giường Ngủ", 14490000, "https://cdn.hstatic.net/products/200000065946/pro_xam_comet_1m6_72759109edb7408f8ac2af5a4c1025a8_grande.png"),
            new("Giường Ngủ Bọc Da Cao Cấp Hiện Đại MOHO BLINK", "Giường Ngủ", 15990000, "https://cdn.hstatic.net/products/200000065946/pro_nau_giuong_blink_1650f9959ce0420fb381f1f963d96757_grande.png"),
            new("Giường Ngủ Bọc Vải Cao Cấp MOHO BOND", "Giường Ngủ", 14990000, "https://cdn.hstatic.net/products/200000065946/pro_xam_bond_7e00691a238c4624923bb7b84b5bbab8_grande.png"),
            new("Giường Ngủ Bọc Vải 1m6 SCARLET - MOHO Signature", "Giường Ngủ", 17990000, "https://cdn.hstatic.net/products/200000065946/pro_nau_giuong_scarlet_noi_that_moho_780bf5061e2648b2a20699c72634a74a_grande.jpg"),
            new("Giường Ngủ Gỗ Tràm MOHO VLINE 601 Nhiều Kích Thước", "Giường Ngủ", 6490000, "https://cdn.hstatic.net/products/200000065946/pro_nau_giuong_go_1m8_vline_noi_that_moho_a10982a101b1467eb2b98199d442e2a0_grande.png"),
            new("Giường Ngủ Gỗ MOHO VLINE 601 Màu Tự Nhiên", "Giường Ngủ", 6490000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_giuong_go_cao_su_vline_noi_that_moho_2_107dda39f9804078aebacbba888ba4a1_grande.png"),

            new("Combo Tủ Quần Áo Cánh Kính MOHO ASTRO", "Tủ Quần Áo", 23490000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_astro__3__ac791f4097644fb7b8a8e095a1acc855_grande.png"),
            new("Tủ Quần Áo Cánh Kính Cao Cấp 1m2 MOHO ASTRO", "Tủ Quần Áo", 14990000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_astro_197b0eef922e45fd96e1759ad391b148_grande.png"),
            new("Tủ Quần Áo Cánh Kính 60cm MOHO ASTRO", "Tủ Quần Áo", 9490000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_tu_quan_ao_canh_kinh_60cm_moho_astro_639fe5bc00d54eeab689398110e9d508_grande.jpg"),
            new("Combo Tủ Quần Áo VLINE V3", "Tủ Quần Áo", 17490000, "https://cdn.hstatic.net/products/200000065946/pro_nau_tu_quan_ao_2m4_vline_noi_that_moho_1_dff0bdf4c5fb44b4b30e28f3ae563f0f_grande.jpg"),
            new("Tủ Quần Áo 2 Cánh VLINE V3", "Tủ Quần Áo", 10490000, "https://cdn.hstatic.net/products/200000065946/pro_nau_tu_quan_ao_vline_2_canh__noi_that_moho_main_f8e321cc421243029b0e2de665a0cf00_grande.jpg"),
            new("Tủ Quần Áo Gỗ Tần Bì Châu Âu SCARLET - MOHO Signature", "Tủ Quần Áo", 18990000, "https://cdn.hstatic.net/products/200000065946/pro_nau_tu_quan_ao_go_tan_bi_scarlet_noi_that_moho_1_c30d3b95d4c24621869aff2c5c2091f3_grande.jpg"),

            new("Tủ Đầu Giường VIENNA (Màu gỗ phối trắng)", "Tủ Đầu Giường", 1690000, "https://cdn.hstatic.net/products/200000065946/pro_mix_tu_dau_giuong_moho_vienna_a27cbbe06280419a89571f625b6594de_grande.png"),
            new("Tủ Đầu Giường Gỗ Tần Bì Cao Cấp SCARLET - MOHO Signature", "Tủ Đầu Giường", 4990000, "https://cdn.hstatic.net/products/200000065946/pro_nau_tu_dau_giuong_scarlet_noi_that_moho_1_0681d05233b142f5a67b600da6be0ca4_grande.jpg"),
            new("Tủ Đầu Giường Gỗ MOHO VLINE 801 Màu Tự Nhiên", "Tủ Đầu Giường", 2490000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_tu_dau_giuong_vline_5_8789ba9c811d4c31a71b47d8eb17744b_grande.jpg"),
            new("Tủ Đầu Giường Gỗ MOHO VLINE 802", "Tủ Đầu Giường", 2490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_tu_dau_giuong_vline_802_f9f7d4f9a419432a914a37abb8c79065_grande.jpg"),
            new("Tủ Đầu Giường Gỗ MOHO HOBRO 301", "Tủ Đầu Giường", 2490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_tu_dau_giuong_go_hobro_6_549626d7accc4eafbffe37bf728f564e_grande.jpg"),
            new("Tủ Đầu Giường MOHO KOSTER Màu Nâu", "Tủ Đầu Giường", 1490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_tu_dau_giuong_moho_koster_mau_nau_5_14946587d32d450e9284c11314511d2a_grande.png"),

            new("Bàn Trang Điểm Gỗ Đa Năng MOHO VIENNA 202 Màu Nâu", "Bàn Trang Điểm", 3990000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_combo_ban_va_guong_trang_diem_vienna_7_f56dbbc1e08a41d49b57bffd2425ad04_large.jpg"),
            new("Bàn Trang Điểm Gỗ Đa Năng MOHO VIENNA 202 Màu Tự Nhiên", "Bàn Trang Điểm", 3990000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_combo_ban_va_guong_trang_diem_2_b1b0519b9dc84f0bba38fc1f0976329f_large.jpg"),

            new("Bàn Sofa - Bàn Cafe - Bàn Trà Dalumd (Màu Nâu Hạnh Nhân, 80)", "Bàn Sofa/Trà", 2490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_ban_dalumd_2_c4892d163e1d4890b5e82c996bc6c7c8_grande.jpg"),
            new("Set Bàn Sofa - Bàn Trà - Bàn Cafe Gỗ KLINE", "Bàn Sofa/Trà", 1990000, "https://product.hstatic.net/200000065946/product/pro_mau_trang_ban_sofa_kline__2__c66b166ceb344dab99cbbdd5fddc6a87_grande.png"),
            new("Bàn Sofa - Bàn Cafe - Bàn Trà Gỗ MOHO VLINE 501", "Bàn Sofa/Trà", 2490000, "https://product.hstatic.net/200000065946/product/pro_nau_ban_cafe_vline_bbaf46aeb9ce49bd9181afeb76309f71_grande.jpg"),
            new("Bàn Sofa MOHO KOSTER Màu Nâu", "Bàn Sofa/Trà", 1790000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_ban_sofa_koster_mau_nau_1_7bd3712bece542ecb45bfe3933373900_grande.png"),
            new("Combo Phòng Khách MOHO VLINE Màu Tự Nhiên", "Bàn Sofa/Trà", 18490000, "https://product.hstatic.net/200000065946/product/pro_combo_mau_tu_nhien_dem_be_noi_that_moho_ca8e5ba4420d426b85f0d3bf82181e62_grande.png"),
            new("Bàn Sofa HOBRO 301 (màu nâu 90)", "Bàn Sofa/Trà", 1990000, "https://product.hstatic.net/200000065946/product/pro_mau_nau_noi_that_moho_ban_sofa_hobro___1__b8b2225e1732409086e953121185762d_grande.jpg"),

            new("Tủ Tivi Dalumd (Màu Nâu Hạnh Nhân, 160)", "Tủ Kệ Tivi", 3990000, "https://cdn.hstatic.net/products/200000065946/pro_nau_noi_that_moho_tutv_dalumd_1_3c471723a37f44129f610e278ba3d54b_grande.png"),
            new("Kệ Tivi Style Hàn KLINE", "Tủ Kệ Tivi", 4490000, "https://product.hstatic.net/200000065946/product/pro_mau_trang_ke_tivi_kline_7a024cfb938644f88114008d2e90565c_grande.jpg"),
            new("Kệ TV MOHO HOBRO 301 (Màu Nâu 180)", "Tủ Kệ Tivi", 4490000, "https://cdn.hstatic.net/products/200000065946/pro_mau_nau_noi_that_moho_ke_tv_hobro_main_67b7e5845d284299bbfb53226c4178af_grande.png"),
            new("Tủ Kệ Tivi Gỗ MOHO OSLO 201", "Tủ Kệ Tivi", 3490000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_tu_tv_oslo_noi_that_moho_1_e8dd0f7970314b82bf6ac4e86c47add9_grande.png"),
            new("Tủ Kệ Tivi Gỗ Tràm MOHO VLINE 301", "Tủ Kệ Tivi", 4490000, "https://product.hstatic.net/200000065946/product/pro_nau_ke_tv_vline_6_a3e957fe30df4c92af99190bd940621f_grande.jpg"),
            new("Tủ Kệ TiVi Gỗ MOHO FIJI 401", "Tủ Kệ Tivi", 3399000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_tu_ke_tivi_fiji_6_a475a39986a04b859d0c8f572ddada4c_grande.jpg"),

            new("Tủ Giày - Tủ Trang Trí Gỗ MOHO OSLO 901", "Tủ Giày - Tủ Trang Trí", 2990000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_tu_giay_oslo_901_2_5f7ed884249f488db0af1cd94536ff38_grande.jpg"),
            new("Tủ Giày - Tủ Trang Trí Gỗ MOHO VLINE 601", "Tủ Giày - Tủ Trang Trí", 3490000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_tu_giay_vline_601_66124fd051e24faa9d0de1521365691c_grande.jpg"),
            new("Tủ Giày - Tủ Trang Trí Gỗ MOHO VIENNA 203", "Tủ Giày - Tủ Trang Trí", 2999000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_tu_giay_trang_tri_vienna_203_0c115ffc755b472c9af7ad171926daab_grande.jpg"),
            new("Tủ Giày - Tủ Trang Trí Gỗ MOHO VIENNA 201", "Tủ Giày - Tủ Trang Trí", 1999000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_tu_giay_trang_tri_vienna_201_3bb47b0145e747d683e905c21e06fb90_grande.jpg"),
            new("Tủ Giày - Tủ Trang Trí Gỗ Tràm MOHO VLINE 602", "Tủ Giày - Tủ Trang Trí", 3490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_tu_giay_vline_601_1_4d815074d997442b86777b129e166f13_grande.jpg"),

            new("Bộ Bàn Ăn Gỗ Tự Nhiên PLANK Veneer Gỗ Sồi", "Bàn Ăn", 8490000, "https://cdn.hstatic.net/products/200000065946/pro_nau_bo_ban_ghe_4_cho_6_cho_plank_noi_that_moho_5_main_7a37814db7ee440480447827ef5b5996_grande.jpg"),
            new("Bàn Ăn Gỗ 1m6 SERENA", "Bàn Ăn", 5990000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_bo_ban_an_4_ghe_6_ghe_serena_noi_that_moho_ban_an_1m6_238807752ee145c19f3e3ccdb43977d6_grande.jpg"),
            new("Bàn Ăn Gỗ Tự Nhiên SCANIA (Màu Nâu, Mặt Vân Đá, 140)", "Bàn Ăn", 4490000, "https://product.hstatic.net/200000065946/product/pro_mau_nau_noi_that_moho_ban___10__dadc7e0aeef74377832bd757065f54be_grande.jpg"),
            new("Bàn Ăn Gỗ Cao Su Tự Nhiên HOBRO 301 (Màu nâu)", "Bàn Ăn", 4490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_ban_an___12__6856b0b5d2ae4e4c9b4394523b200e2d_grande.jpg"),
            new("Bàn Ăn Gỗ MOHO NARVIK 1m2", "Bàn Ăn", 2490000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_ban_an___7__3eb6f54ac7af49b58fd34a01b4ae44c8_grande.jpg"),
            new("Bàn Ăn Gỗ MOHO KOSTER Màu Nâu", "Bàn Ăn", 3490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_ban_an_go_moho_koster_mau_nau_11_d3c38aafb0934bb08a51621f65176874_grande.png"),

            new("Ghế Gỗ HERNING Lưng Mây Đan", "Ghế Ăn", 1990000, "https://cdn.hstatic.net/products/200000065946/pro_nau_ghe_go_herning_lung_may_noi_that_moho_2_19c7c982d7b3418c9f168d176ef282af_grande.png"),
            new("Ghế Ăn Bọc Đệm FINGAL", "Ghế Ăn", 1990000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_ghe_go_fingal_noi_that_moho_2_6ebff4469236454eb5967d6103c0e5e1_grande.png"),
            new("Ghế Ăn Gỗ Cao Su Tự Nhiên SERENA", "Ghế Ăn", 1990000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_bo_ban_an_4_ghe_6_ghe_serena_noi_that_moho_53a48ebc71f04d4ea4a1e599ccdfa86e_grande.jpg"),
            new("Ghế Gỗ LYH - Đệm Lưng Tháo Rời", "Ghế Ăn", 2490000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_ghe_go_lyh_noi_that_moho_a9e8073e5cd04d3f9a25e141859496b3_grande.png"),
            new("Ghế Ăn Gỗ Cao Su Tự Nhiên HOBRO 301", "Ghế Ăn", 1990000, "https://product.hstatic.net/200000065946/product/pro_mau_nau_noi_that_moho_ghe_an___11__21e6f141ea7b4797836239063f8215e5_grande.jpg"),
            new("Ghế Bàn Ăn Gỗ Tự Nhiên PLANK", "Ghế Ăn", 1990000, "https://cdn.hstatic.net/products/200000065946/pro_mau_tu_nhien_ghe_go_plank_noi_that_moho_d8a3adeeb49548589f02a236a7931a30_grande.jpg"),

            new("Bàn Làm Việc Gỗ MOHO VLINE 601 Màu Nâu", "Bàn Làm Việc", 2490000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_ban_lam_viec_vline_601_a_e60e2f8b72854311ae12424eed3cb88a_grande.jpg"),
            new("Bàn Làm Việc Gỗ MOHO FYN 601 Màu Nâu", "Bàn Làm Việc", 2990000, "https://product.hstatic.net/200000065946/product/pro_nau_noi_that_moho_ban_lam_viec_go_fyn_nau2_f607075e46254fc190dfabcd5108f91c_grande.jpg"),
            new("Bàn Làm Việc Gỗ MOHO VLINE 601 Màu Tự Nhiên", "Bàn Làm Việc", 2490000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_ban_lam_viec_vline_1_92060b73c393469181d9d24218c38851_grande.jpg"),
            new("Bàn Máy Tính Gỗ MOHO WORKS 702 - 1m2", "Bàn Làm Việc", 1799000, "https://product.hstatic.net/200000065946/product/pro_trang_noi_that_moho_ban_may_tinh_1_07121e2728134e579bdfd34fd6e3c184_grande.jpg"),
            new("Bàn Làm Việc Gỗ MOHO FYN 601 Màu Tự Nhiên", "Bàn Làm Việc", 2990000, "https://product.hstatic.net/200000065946/product/pro_mau_tu_nhien_noi_that_moho_ban_lam_viec_go_fyn_1_363104d4c8da475394e683e556879f88_grande.jpg"),
        };
    }
}
