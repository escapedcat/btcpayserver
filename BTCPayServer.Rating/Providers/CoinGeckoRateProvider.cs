using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Rating;
using Newtonsoft.Json.Linq;

namespace BTCPayServer.Services.Rates
{
    public class CoinGeckoRateProvider : IRateProvider, IHasExchangeName
    {
        // https://api.coingecko.com/api/v3/exchanges/list
        internal static readonly string SupportedExchanges = "[{\"id\":\"abcc\",\"name\":\"ABCC\"},{\"id\":\"acx\",\"name\":\"ACX\"},{\"id\":\"aex\",\"name\":\"AEX\"},{\"id\":\"airswap\",\"name\":\"AirSwap\"},{\"id\":\"allbit\",\"name\":\"Allbit\"},{\"id\":\"allcoin\",\"name\":\"Allcoin\"},{\"id\":\"alterdice\",\"name\":\"AlterDice\"},{\"id\":\"altilly\",\"name\":\"Altilly\"},{\"id\":\"altmarkets\",\"name\":\"Altmarkets\"},{\"id\":\"anx\",\"name\":\"ANX\"},{\"id\":\"aphelion\",\"name\":\"Aphelion\"},{\"id\":\"atomars\",\"name\":\"Atomars\"},{\"id\":\"axnet\",\"name\":\"AXNET\"},{\"id\":\"b2bx\",\"name\":\"B2BX\"},{\"id\":\"bakkt\",\"name\":\"Bakkt\"},{\"id\":\"bamboo_relay\",\"name\":\"Bamboo Relay\"},{\"id\":\"bancor\",\"name\":\"Bancor Network\"},{\"id\":\"bankera\",\"name\":\"Bankera\"},{\"id\":\"basefex\",\"name\":\"BaseFEX\"},{\"id\":\"bcex\",\"name\":\"BCEX\"},{\"id\":\"beaxy\",\"name\":\"Beaxy\"},{\"id\":\"bgogo\",\"name\":\"Bgogo\"},{\"id\":\"bhex\",\"name\":\"BHEX\"},{\"id\":\"bibox\",\"name\":\"Bibox\"},{\"id\":\"bibox_futures\",\"name\":\"Bibox (Futures)\"},{\"id\":\"bigmarkets\",\"name\":\"BIG markets\"},{\"id\":\"bigone\",\"name\":\"BigONE\"},{\"id\":\"bihodl\",\"name\":\"BiHODL \"},{\"id\":\"biki\",\"name\":\"Biki\"},{\"id\":\"bilaxy\",\"name\":\"Bilaxy\"},{\"id\":\"binance\",\"name\":\"Binance\"},{\"id\":\"binance_dex\",\"name\":\"Binance DEX\"},{\"id\":\"binance_futures\",\"name\":\"Binance (Futures)\"},{\"id\":\"binance_jersey\",\"name\":\"Binance Jersey\"},{\"id\":\"binance_us\",\"name\":\"Binance US\"},{\"id\":\"bione\",\"name\":\"Bione\"},{\"id\":\"birake\",\"name\":\"Birake\"},{\"id\":\"bisq\",\"name\":\"Bisq\"},{\"id\":\"bit2c\",\"name\":\"Bit2c\"},{\"id\":\"bitalong\",\"name\":\"Bitalong\"},{\"id\":\"bitasset\",\"name\":\"BitAsset\"},{\"id\":\"bitbank\",\"name\":\"Bitbank\"},{\"id\":\"bitbay\",\"name\":\"BitBay\"},{\"id\":\"bitbegin\",\"name\":\"Bitbegin\"},{\"id\":\"bitbox\",\"name\":\"BITBOX\"},{\"id\":\"bitc3\",\"name\":\"Bitc3\"},{\"id\":\"bitci\",\"name\":\"Bitci\"},{\"id\":\"bitcoin_com\",\"name\":\"Bitcoin.com\"},{\"id\":\"bitcratic\",\"name\":\"Bitcratic\"},{\"id\":\"bitex\",\"name\":\"Bitex.la\"},{\"id\":\"bitexbook\",\"name\":\"BITEXBOOK\"},{\"id\":\"bitexlive\",\"name\":\"Bitexlive\"},{\"id\":\"bitfex\",\"name\":\"Bitfex\"},{\"id\":\"bitfinex\",\"name\":\"Bitfinex\"},{\"id\":\"bitfinex_futures\",\"name\":\"Bitfinex (Futures)\"},{\"id\":\"bitflyer\",\"name\":\"bitFlyer\"},{\"id\":\"bitflyer_futures\",\"name\":\"Bitflyer (Futures)\"},{\"id\":\"bitforex\",\"name\":\"Bitforex\"},{\"id\":\"bitforex_futures\",\"name\":\"Bitforex (Futures)\"},{\"id\":\"bithash\",\"name\":\"BitHash\"},{\"id\":\"bitholic\",\"name\":\"Bithumb Singapore\"},{\"id\":\"bithumb\",\"name\":\"Bithumb\"},{\"id\":\"bithumb_global\",\"name\":\"Bithumb Global\"},{\"id\":\"bitinfi\",\"name\":\"Bitinfi\"},{\"id\":\"bitker\",\"name\":\"BITKER\"},{\"id\":\"bitkonan\",\"name\":\"BitKonan\"},{\"id\":\"bitkub\",\"name\":\"Bitkub\"},{\"id\":\"bitlish\",\"name\":\"Bitlish\"},{\"id\":\"bitmart\",\"name\":\"BitMart\"},{\"id\":\"bitmax\",\"name\":\"BitMax\"},{\"id\":\"bitmesh\",\"name\":\"Bitmesh\"},{\"id\":\"bitmex\",\"name\":\"Bitmex\"},{\"id\":\"bitoffer\",\"name\":\"Bitoffer\"},{\"id\":\"bitonbay\",\"name\":\"BitOnBay\"},{\"id\":\"bitopro\",\"name\":\"BitoPro\"},{\"id\":\"bitpanda\",\"name\":\"Bitpanda Global Exchange\"},{\"id\":\"bitrabbit\",\"name\":\"BitRabbit\"},{\"id\":\"bitrue\",\"name\":\"Bitrue\"},{\"id\":\"bits_blockchain\",\"name\":\"Bits Blockchain\"},{\"id\":\"bitsdaq\",\"name\":\"Bitsdaq\"},{\"id\":\"bitshares_assets\",\"name\":\"Bitshares Assets\"},{\"id\":\"bitso\",\"name\":\"Bitso\"},{\"id\":\"bitsonic\",\"name\":\"Bitsonic\"},{\"id\":\"bitstamp\",\"name\":\"Bitstamp\"},{\"id\":\"bitsten\",\"name\":\"Bitsten\"},{\"id\":\"bitstorage\",\"name\":\"BitStorage\"},{\"id\":\"bittrex\",\"name\":\"Bittrex\"},{\"id\":\"bit_z\",\"name\":\"Bit-Z\"},{\"id\":\"bitz_futures\",\"name\":\"Bitz (Futures)\"},{\"id\":\"bkex\",\"name\":\"BKEX\"},{\"id\":\"bleutrade\",\"name\":\"bleutrade\"},{\"id\":\"blockonix\",\"name\":\"Blockonix\"},{\"id\":\"boa\",\"name\":\"BOA Exchange\"},{\"id\":\"braziliex\",\"name\":\"Braziliex\"},{\"id\":\"btc_alpha\",\"name\":\"BTC-Alpha\"},{\"id\":\"btcbox\",\"name\":\"BTCBOX\"},{\"id\":\"btcc\",\"name\":\"BTCC\"},{\"id\":\"btcexa\",\"name\":\"BTCEXA\"},{\"id\":\"btc_exchange\",\"name\":\"Btc Exchange\"},{\"id\":\"btcmarkets\",\"name\":\"BTCMarkets\"},{\"id\":\"btcnext\",\"name\":\"BTCNEXT\"},{\"id\":\"btcsquare\",\"name\":\"BTCSquare\"},{\"id\":\"btc_trade_ua\",\"name\":\"BTC Trade UA\"},{\"id\":\"btcturk\",\"name\":\"BTCTurk\"},{\"id\":\"btse\",\"name\":\"BTSE\"},{\"id\":\"btse_futures\",\"name\":\"BTSE (Futures)\"},{\"id\":\"buyucoin\",\"name\":\"BuyUcoin\"},{\"id\":\"bvnex\",\"name\":\"Bvnex\"},{\"id\":\"bw\",\"name\":\"BW.com\"},{\"id\":\"bx_thailand\",\"name\":\"BX Thailand\"},{\"id\":\"bybit\",\"name\":\"Bybit\"},{\"id\":\"c2cx\",\"name\":\"C2CX\"},{\"id\":\"cashierest\",\"name\":\"Cashierest\"},{\"id\":\"cashpayz\",\"name\":\"Cashpayz\"},{\"id\":\"catex\",\"name\":\"Catex\"},{\"id\":\"cbx\",\"name\":\"CBX\"},{\"id\":\"ccex\",\"name\":\"C-CEX\"},{\"id\":\"ccx\",\"name\":\"CCXCanada\"},{\"id\":\"cex\",\"name\":\"CEX.IO\"},{\"id\":\"cezex\",\"name\":\"Cezex\"},{\"id\":\"chainex\",\"name\":\"ChainEX\"},{\"id\":\"chainrift\",\"name\":\"Chainrift\"},{\"id\":\"chaoex\",\"name\":\"CHAOEX\"},{\"id\":\"citex\",\"name\":\"CITEX\"},{\"id\":\"cme_futures\",\"name\":\"CME Bitcoin Futures\"},{\"id\":\"codex\",\"name\":\"CODEX\"},{\"id\":\"coinall\",\"name\":\"CoinAll\"},{\"id\":\"coinasset\",\"name\":\"CoinAsset\"},{\"id\":\"coinbe\",\"name\":\"Coinbe\"},{\"id\":\"coinbene\",\"name\":\"CoinBene\"},{\"id\":\"coinbig\",\"name\":\"COINBIG\"},{\"id\":\"coinbit\",\"name\":\"Coinbit\"},{\"id\":\"coinchangex\",\"name\":\"Coinchangex\"},{\"id\":\"coincheck\",\"name\":\"Coincheck\"},{\"id\":\"coindeal\",\"name\":\"Coindeal\"},{\"id\":\"coindirect\",\"name\":\"CoinDirect\"},{\"id\":\"coineal\",\"name\":\"Coineal\"},{\"id\":\"coin_egg\",\"name\":\"CoinEgg\"},{\"id\":\"coinex\",\"name\":\"CoinEx\"},{\"id\":\"coinfalcon\",\"name\":\"Coinfalcon\"},{\"id\":\"coinfield\",\"name\":\"Coinfield\"},{\"id\":\"coinfinit\",\"name\":\"Coinfinit\"},{\"id\":\"coinflex\",\"name\":\"CoinFLEX\"},{\"id\":\"coinflex_futures\",\"name\":\"CoinFLEX (Futures)\"},{\"id\":\"coinfloor\",\"name\":\"Coinfloor\"},{\"id\":\"coingi\",\"name\":\"Coingi\"},{\"id\":\"coinhe\",\"name\":\"CoinHe\"},{\"id\":\"coinhub\",\"name\":\"Coinhub\"},{\"id\":\"coinjar\",\"name\":\"CoinJar Exchange\"},{\"id\":\"coinlim\",\"name\":\"Coinlim\"},{\"id\":\"coin_metro\",\"name\":\"Coinmetro\"},{\"id\":\"coinmex\",\"name\":\"CoinMex\"},{\"id\":\"coinnest\",\"name\":\"CoinNest\"},{\"id\":\"coinone\",\"name\":\"Coinone\"},{\"id\":\"coinpark\",\"name\":\"Coinpark\"},{\"id\":\"coinplace\",\"name\":\"Coinplace\"},{\"id\":\"coinsbank\",\"name\":\"Coinsbank\"},{\"id\":\"coinsbit\",\"name\":\"Coinsbit\"},{\"id\":\"coinsuper\",\"name\":\"Coinsuper\"},{\"id\":\"cointiger\",\"name\":\"CoinTiger\"},{\"id\":\"coinxpro\",\"name\":\"COINX.PRO\"},{\"id\":\"coinzest\",\"name\":\"Coinzest\"},{\"id\":\"coinzo\",\"name\":\"Coinzo\"},{\"id\":\"c_patex\",\"name\":\"C-Patex\"},{\"id\":\"cpdax\",\"name\":\"CPDAX\"},{\"id\":\"credoex\",\"name\":\"CredoEx\"},{\"id\":\"crex24\",\"name\":\"CREX24\"},{\"id\":\"crxzone\",\"name\":\"CRXzone\"},{\"id\":\"cryptaldash\",\"name\":\"CryptalDash\"},{\"id\":\"cryptex\",\"name\":\"Cryptex\"},{\"id\":\"crypto_bridge\",\"name\":\"CryptoBridge\"},{\"id\":\"cryptology\",\"name\":\"Cryptology\"},{\"id\":\"cryptonit\",\"name\":\"Cryptonit\"},{\"id\":\"crytrex\",\"name\":\"CryTrEx\"},{\"id\":\"cybex\",\"name\":\"Cybex DEX\"},{\"id\":\"dach_exchange\",\"name\":\"Dach Exchange\"},{\"id\":\"dakuce\",\"name\":\"Dakuce\"},{\"id\":\"darb_finance\",\"name\":\"Darb Finance\"},{\"id\":\"daybit\",\"name\":\"Daybit\"},{\"id\":\"dcoin\",\"name\":\"Dcoin\"},{\"id\":\"ddex\",\"name\":\"DDEX\"},{\"id\":\"decoin\",\"name\":\"Decoin\"},{\"id\":\"delta_futures\",\"name\":\"Delta Exchange\"},{\"id\":\"deribit\",\"name\":\"Deribit\"},{\"id\":\"dextop\",\"name\":\"DEx.top\"},{\"id\":\"dextrade\",\"name\":\"Dex-Trade\"},{\"id\":\"dflow\",\"name\":\"Dflow\"},{\"id\":\"digifinex\",\"name\":\"Digifinex\"},{\"id\":\"digitalprice\",\"name\":\"Altsbit\"},{\"id\":\"dobitrade\",\"name\":\"Dobitrade\"},{\"id\":\"dove_wallet\",\"name\":\"Dove Wallet\"},{\"id\":\"dragonex\",\"name\":\"DragonEx\"},{\"id\":\"dsx\",\"name\":\"DSX\"},{\"id\":\"dydx\",\"name\":\"dYdX\"},{\"id\":\"ecxx\",\"name\":\"Ecxx\"},{\"id\":\"elitex\",\"name\":\"Elitex\"},{\"id\":\"eosex\",\"name\":\"EOSex\"},{\"id\":\"escodex\",\"name\":\"Escodex\"},{\"id\":\"eterbase\",\"name\":\"Eterbase\"},{\"id\":\"etherflyer\",\"name\":\"EtherFlyer\"},{\"id\":\"ethex\",\"name\":\"Ethex\"},{\"id\":\"everbloom\",\"name\":\"Everbloom\"},{\"id\":\"exmarkets\",\"name\":\"ExMarkets\"},{\"id\":\"exmo\",\"name\":\"EXMO\"},{\"id\":\"exnce\",\"name\":\"EXNCE\"},{\"id\":\"exrates\",\"name\":\"Exrates\"},{\"id\":\"extstock\",\"name\":\"ExtStock\"},{\"id\":\"exx\",\"name\":\"EXX\"},{\"id\":\"f1cx\",\"name\":\"F1CX\"},{\"id\":\"fatbtc\",\"name\":\"FatBTC\"},{\"id\":\"fcoin\",\"name\":\"FCoin\"},{\"id\":\"fex\",\"name\":\"FEX\"},{\"id\":\"financex\",\"name\":\"FinanceX\"},{\"id\":\"finexbox\",\"name\":\"FinexBox\"},{\"id\":\"fisco\",\"name\":\"Fisco\"},{\"id\":\"floatsv\",\"name\":\"Float SV\"},{\"id\":\"fmex\",\"name\":\"FMex\"},{\"id\":\"forkdelta\",\"name\":\"ForkDelta\"},{\"id\":\"freiexchange\",\"name\":\"Freiexchange\"},{\"id\":\"ftx\",\"name\":\"FTX\"},{\"id\":\"ftx_spot\",\"name\":\"FTX (Spot)\"},{\"id\":\"fubt\",\"name\":\"FUBT\"},{\"id\":\"gate\",\"name\":\"Gate.io\"},{\"id\":\"gate_futures\",\"name\":\"Gate.io (Futures)\"},{\"id\":\"gbx\",\"name\":\"Gibraltar Blockchain Exchange\"},{\"id\":\"gdac\",\"name\":\"GDAC\"},{\"id\":\"gdax\",\"name\":\"Coinbase Pro\"},{\"id\":\"gemini\",\"name\":\"Gemini\"},{\"id\":\"getbtc\",\"name\":\"GetBTC\"},{\"id\":\"gmo_japan\",\"name\":\"GMO Japan\"},{\"id\":\"gmo_japan_futures\",\"name\":\"GMO Japan (Futures)\"},{\"id\":\"gobaba\",\"name\":\"Gobaba\"},{\"id\":\"go_exchange\",\"name\":\"Go Exchange\"},{\"id\":\"gopax\",\"name\":\"GoPax\"},{\"id\":\"graviex\",\"name\":\"Graviex\"},{\"id\":\"hanbitco\",\"name\":\"Hanbitco\"},{\"id\":\"hb_top\",\"name\":\"Hb.top\"},{\"id\":\"hitbtc\",\"name\":\"HitBTC\"},{\"id\":\"hotbit\",\"name\":\"Hotbit\"},{\"id\":\"hpx\",\"name\":\"HPX\"},{\"id\":\"hubi\",\"name\":\"Hubi\"},{\"id\":\"huobi\",\"name\":\"Huobi Global\"},{\"id\":\"huobi_dm\",\"name\":\"Huobi DM\"},{\"id\":\"huobi_japan\",\"name\":\"Huobi Japan\"},{\"id\":\"huobi_korea\",\"name\":\"Huobi Korea\"},{\"id\":\"huobi_us\",\"name\":\"Huobi US (HBUS)\"},{\"id\":\"ice3x\",\"name\":\"Ice3x\"},{\"id\":\"idcm\",\"name\":\"IDCM\"},{\"id\":\"idex\",\"name\":\"Idex\"},{\"id\":\"incorex\",\"name\":\"IncoreX\"},{\"id\":\"independent_reserve\",\"name\":\"Independent Reserve\"},{\"id\":\"indodax\",\"name\":\"Indodax\"},{\"id\":\"indoex\",\"name\":\"Indoex\"},{\"id\":\"infinity_coin\",\"name\":\"Infinity Coin\"},{\"id\":\"instantbitex\",\"name\":\"Instant Bitex\"},{\"id\":\"iqfinex\",\"name\":\"IQFinex\"},{\"id\":\"ironex\",\"name\":\"Ironex\"},{\"id\":\"itbit\",\"name\":\"itBit\"},{\"id\":\"jex\",\"name\":\"Binance JEX\"},{\"id\":\"jex_futures\",\"name\":\"Binance JEX (Futures)\"},{\"id\":\"joyso\",\"name\":\"Joyso\"},{\"id\":\"kairex\",\"name\":\"KAiREX\"},{\"id\":\"kkcoin\",\"name\":\"KKCoin\"},{\"id\":\"k_kex\",\"name\":\"KKEX\"},{\"id\":\"koinok\",\"name\":\"Koinok\"},{\"id\":\"koinx\",\"name\":\"Koinx\"},{\"id\":\"korbit\",\"name\":\"Korbit\"},{\"id\":\"kraken\",\"name\":\"Kraken\"},{\"id\":\"kraken_futures\",\"name\":\"Kraken (Futures)\"},{\"id\":\"kryptono\",\"name\":\"Kryptono\"},{\"id\":\"kucoin\",\"name\":\"KuCoin\"},{\"id\":\"kumex\",\"name\":\"Kumex\"},{\"id\":\"kuna\",\"name\":\"Kuna Exchange\"},{\"id\":\"kyber_network\",\"name\":\"Kyber Network\"},{\"id\":\"lakebtc\",\"name\":\"LakeBTC\"},{\"id\":\"latoken\",\"name\":\"LATOKEN\"},{\"id\":\"lbank\",\"name\":\"LBank\"},{\"id\":\"letsdocoinz\",\"name\":\"Letsdocoinz\"},{\"id\":\"livecoin\",\"name\":\"Livecoin\"},{\"id\":\"localtrade\",\"name\":\"LocalTrade\"},{\"id\":\"lukki\",\"name\":\"Lukki\"},{\"id\":\"luno\",\"name\":\"Luno\"},{\"id\":\"lykke\",\"name\":\"Lykke\"},{\"id\":\"mandala\",\"name\":\"Mandala\"},{\"id\":\"max_maicoin\",\"name\":\"Max Maicoin\"},{\"id\":\"mercado_bitcoin\",\"name\":\"Mercado Bitcoin\"},{\"id\":\"mercatox\",\"name\":\"Mercatox\"},{\"id\":\"mercuriex\",\"name\":\"MercuriEx\"},{\"id\":\"mxc\",\"name\":\"MXC\"},{\"id\":\"nanu_exchange\",\"name\":\"Nanu Exchange\"},{\"id\":\"nash\",\"name\":\"Nash\"},{\"id\":\"neblidex\",\"name\":\"Neblidex\"},{\"id\":\"negociecoins\",\"name\":\"Negociecoins\"},{\"id\":\"neraex\",\"name\":\"Neraex\"},{\"id\":\"newdex\",\"name\":\"Newdex\"},{\"id\":\"nexybit\",\"name\":\"Nexybit\"},{\"id\":\"ninecoin\",\"name\":\"9coin\"},{\"id\":\"nlexch\",\"name\":\"NLexch\"},{\"id\":\"novadax\",\"name\":\"NovaDAX\"},{\"id\":\"novadex\",\"name\":\"Novadex\"},{\"id\":\"oasis_trade\",\"name\":\"OasisDEX\"},{\"id\":\"oceanex\",\"name\":\"Oceanex\"},{\"id\":\"oex\",\"name\":\"OEX\"},{\"id\":\"okcoin\",\"name\":\"OKCoin\"},{\"id\":\"okex\",\"name\":\"OKEx\"},{\"id\":\"okex_korea\",\"name\":\"OKEx Korea\"},{\"id\":\"okex_swap\",\"name\":\"OKEx (Futures)\"},{\"id\":\"omgfin\",\"name\":\"Omgfin\"},{\"id\":\"omnitrade\",\"name\":\"OmniTrade\"},{\"id\":\"ooobtc\",\"name\":\"OOOBTC\"},{\"id\":\"openledger\",\"name\":\"OpenLedger DEX\"},{\"id\":\"orderbook\",\"name\":\"Orderbook.io\"},{\"id\":\"ore_bz\",\"name\":\"Ore BZ\"},{\"id\":\"otcbtc\",\"name\":\"OTCBTC\"},{\"id\":\"ovex\",\"name\":\"Ovex\"},{\"id\":\"p2pb2b\",\"name\":\"P2PB2B\"},{\"id\":\"paribu\",\"name\":\"Paribu\"},{\"id\":\"paroexchange\",\"name\":\"Paro Exchange\"},{\"id\":\"paymium\",\"name\":\"Paymium\"},{\"id\":\"piexgo\",\"name\":\"Piexgo\"},{\"id\":\"poloniex\",\"name\":\"Poloniex\"},{\"id\":\"prime_xbt\",\"name\":\"Prime XBT\"},{\"id\":\"probit\",\"name\":\"Probit\"},{\"id\":\"purcow\",\"name\":\"Purcow\"},{\"id\":\"qbtc\",\"name\":\"QBTC\"},{\"id\":\"qtrade\",\"name\":\"qTrade\"},{\"id\":\"quoine\",\"name\":\"Liquid\"},{\"id\":\"radar_relay\",\"name\":\"Radar Relay\"},{\"id\":\"raidofinance\",\"name\":\"Raidofinance\"},{\"id\":\"raisex\",\"name\":\"Raisex\"},{\"id\":\"resfinex\",\"name\":\"Resfinex\"},{\"id\":\"rfinex\",\"name\":\"Rfinex\"},{\"id\":\"safe_trade\",\"name\":\"SafeTrade\"},{\"id\":\"satoexchange\",\"name\":\"SatoExchange\"},{\"id\":\"sato_wallet_ex\",\"name\":\"SatowalletEx\"},{\"id\":\"saturn_network\",\"name\":\"Saturn Network\"},{\"id\":\"secondbtc\",\"name\":\"SecondBTC\"},{\"id\":\"shortex\",\"name\":\"Shortex\"},{\"id\":\"simex\",\"name\":\"Simex\"},{\"id\":\"sistemkoin\",\"name\":\"Sistemkoin\"},{\"id\":\"six_x\",\"name\":\"6x\"},{\"id\":\"south_xchange\",\"name\":\"SouthXchange\"},{\"id\":\"stake_cube\",\"name\":\"StakeCube Exchange\"},{\"id\":\"stellar_term\",\"name\":\"StellarTerm\"},{\"id\":\"stocks_exchange\",\"name\":\"STEX\"},{\"id\":\"swiftex\",\"name\":\"Swiftex\"},{\"id\":\"switcheo\",\"name\":\"Switcheo\"},{\"id\":\"syex\",\"name\":\"Shangya Exchange\"},{\"id\":\"synthetix\",\"name\":\"Synthetix Exchange\"},{\"id\":\"tdax\",\"name\":\"Satang Pro\"},{\"id\":\"therocktrading\",\"name\":\"TheRockTrading\"},{\"id\":\"thetokenstore\",\"name\":\"Token.Store\"},{\"id\":\"thinkbit\",\"name\":\"ThinkBit Pro\"},{\"id\":\"three_xbit\",\"name\":\"3XBIT\"},{\"id\":\"tidebit\",\"name\":\"Tidebit\"},{\"id\":\"tidex\",\"name\":\"Tidex\"},{\"id\":\"tokenize\",\"name\":\"Tokenize\"},{\"id\":\"tokenjar\",\"name\":\"TokenJar\"},{\"id\":\"tokenomy\",\"name\":\"Tokenomy\"},{\"id\":\"tokens_net\",\"name\":\"TokensNet\"},{\"id\":\"toko_crypto\",\"name\":\"TokoCrypto\"},{\"id\":\"tokok\",\"name\":\"TOKOK\"},{\"id\":\"tokpie\",\"name\":\"Tokpie\"},{\"id\":\"topbtc\",\"name\":\"TopBTC\"},{\"id\":\"tradeio\",\"name\":\"Trade.io\"},{\"id\":\"trade_ogre\",\"name\":\"TradeOgre\"},{\"id\":\"trade_satoshi\",\"name\":\"Trade Satoshi\"},{\"id\":\"troca_ninja\",\"name\":\"Troca.Ninja\"},{\"id\":\"tron_trade\",\"name\":\"TronTrade\"},{\"id\":\"trx_market\",\"name\":\"PoloniDEX\"},{\"id\":\"tux_exchange\",\"name\":\"Tux Exchange\"},{\"id\":\"txbit\",\"name\":\"Txbit\"},{\"id\":\"uex\",\"name\":\"UEX\"},{\"id\":\"uniswap\",\"name\":\"Uniswap\"},{\"id\":\"unnamed\",\"name\":\"Unnamed\"},{\"id\":\"upbit\",\"name\":\"Upbit\"},{\"id\":\"upbit_indonesia\",\"name\":\"Upbit Indonesia \"},{\"id\":\"vb\",\"name\":\"VB\"},{\"id\":\"vbitex\",\"name\":\"Vbitex\"},{\"id\":\"vcc\",\"name\":\"VCC Exchange\"},{\"id\":\"vebitcoin\",\"name\":\"Vebitcoin\"},{\"id\":\"velic\",\"name\":\"Velic\"},{\"id\":\"vindax\",\"name\":\"Vindax\"},{\"id\":\"vinex\",\"name\":\"Vinex\"},{\"id\":\"vitex\",\"name\":\"ViteX\"},{\"id\":\"waves\",\"name\":\"Waves.Exchange\"},{\"id\":\"wazirx\",\"name\":\"WazirX\"},{\"id\":\"whale_ex\",\"name\":\"WhaleEx\"},{\"id\":\"whitebit\",\"name\":\"Whitebit\"},{\"id\":\"worldcore\",\"name\":\"Worldcore\"},{\"id\":\"xfutures\",\"name\":\"xFutures\"},{\"id\":\"xt\",\"name\":\"XT\"},{\"id\":\"yobit\",\"name\":\"YoBit\"},{\"id\":\"yunex\",\"name\":\"Yunex.io\"},{\"id\":\"zaif\",\"name\":\"Zaif\"},{\"id\":\"zb\",\"name\":\"ZB\"},{\"id\":\"zbg\",\"name\":\"ZBG\"},{\"id\":\"zbmega\",\"name\":\"ZB Mega\"},{\"id\":\"zebpay\",\"name\":\"Zebpay\"},{\"id\":\"zg\",\"name\":\"ZG.com\"},{\"id\":\"zgtop\",\"name\":\"ZG.TOP\"}]";
        private readonly HttpClient Client;
        public static string CoinGeckoName { get; } = "coingecko";
        public string Exchange { get; set; }
        public string ExchangeName => Exchange ?? CoinGeckoName;

        public bool CoinGeckoRate
        {
            get
            {
                return ExchangeName == CoinGeckoName;
            }
        }

        public CoinGeckoRateProvider(IHttpClientFactory httpClientFactory)
        {
            if (httpClientFactory == null)
            {
                return;;
            }
            Client = httpClientFactory.CreateClient();
            Client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public virtual Task<ExchangeRates> GetRatesAsync(CancellationToken cancellationToken)
        {
            return CoinGeckoRate ? GetCoinGeckoRates(cancellationToken) : GetCoinGeckoExchangeSpecificRates(1, cancellationToken);
        }

        private async Task<ExchangeRates> GetCoinGeckoRates(CancellationToken cancellationToken)
        {
            using var resp = await GetWithBackoffAsync("exchange_rates", cancellationToken);
            resp.EnsureSuccessStatusCode();
            return new ExchangeRates(JObject.Parse(await resp.Content.ReadAsStringAsync()).GetValue("rates").Children()
                .Where(token => ((JProperty)token).Name != "btc")
                .Select(token => new ExchangeRate(CoinGeckoName,
                    new CurrencyPair("BTC", ((JProperty)token).Name.ToString()),
                    new BidAsk(((JProperty)token).Value["value"].Value<decimal>()))));
        }

        private async Task<HttpResponseMessage> GetWithBackoffAsync(string request, CancellationToken cancellationToken)
        {
            TimeSpan retryWait = TimeSpan.FromSeconds(1);
retry:
            var resp = await Client.GetAsync(request, cancellationToken);
            if (resp.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                resp.Dispose();
                if (retryWait < TimeSpan.FromSeconds(60))
                {
                    await Task.Delay(retryWait, cancellationToken);
                    retryWait = TimeSpan.FromSeconds(retryWait.TotalSeconds * 2);
                    goto retry;
                }
                resp.EnsureSuccessStatusCode();
            }
            return resp;
        }

        private async Task<ExchangeRates> GetCoinGeckoExchangeSpecificRates(int page, CancellationToken cancellationToken)
        {
            using var resp = await GetWithBackoffAsync($"exchanges/{Exchange}/tickers?page={page}", cancellationToken);

            resp.EnsureSuccessStatusCode();
            List<ExchangeRate> result = JObject.Parse(await resp.Content.ReadAsStringAsync()).GetValue("tickers")
                .Select(token => new ExchangeRate(ExchangeName,
                    new CurrencyPair(token.Value<string>("base"), token.Value<string>("target")),
                    new BidAsk(token.Value<decimal>("last")))).ToList();
            if (page == 1 && resp.Headers.TryGetValues("total", out var total) &&
                resp.Headers.TryGetValues("per-page", out var perPage))
            {
                var totalItems = int.Parse(total.First());
                var perPageItems = int.Parse(perPage.First());

                var totalPages = totalItems / perPageItems;
                if (totalItems % perPageItems != 0)
                {
                    totalPages++;
                }

                var tasks = new List<Task<ExchangeRates>>();
                for (int i = 2; i <= totalPages; i++)
                {
                    tasks.Add(GetCoinGeckoExchangeSpecificRates(i, cancellationToken));
                }

                foreach (var t in (await Task.WhenAll(tasks)))
                {
                    result.AddRange(t);
                }
            }

            return new ExchangeRates(result);
        }
    }
}
